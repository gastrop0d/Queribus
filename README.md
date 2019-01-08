# Queribus
Queribus is a query dispatcher for Unity. It allows you to poll your codebase for numerical collations (sums, min, max), boolean collations (ORs/ANDs) or votes.

Inspired by [Unibus](https://github.com/mattak/Unibus), which is a great companion to Queribus.

## Why Queribus
It can be difficult to maintain decoupling when evaluating expressions in large codebases, particularly when factors for an expression come from many different sources in the codebase.

Typically something asking a question doesn't care about where the answer comes from, it just wants an answer. This can be exploited to greatly decouple areas of a codebase that need to make decisions based on other areas' states, but otherwise don't need to interact.

Query dispatching is a pattern that effectively decouples questions from the sources of their answers.

You can read more about my motivations for query dispatching [here](https://moltenmetalgames.wordpress.com/2018/02/09/critting-with-query-dispatchers/).

## Architecture
![Queribus system diagram](https://moltenmetalgames.files.wordpress.com/2018/02/querydispatcher-1.png)

Each query has three components: a query type, a name and a subject.

- The **query type** determines what data type the query will operate on (boolean, int, float, vote) and, if a boolean or number, what collation operation it will perform (OR/AND, Sum/Min/Max).
- The **name** is a unique string used to identify the query.
- The **subject** is an object of a selected type that is passed to every subscriber when the query is dispatched. It's role is to provide context for the question being asked.

Both the name of the query and the data type of it's subject uniquely identify that query.

## Usage
1. Create a Queribus object in the scene.

2. Implement a query subscriber.

3. Implement a query dispatcher.

# Examples

## Compositing a boolean
Queribus can either OR or AND boolean terms together that it has collected from subscribers. Each subscriber will answer true or false to the query, and the dispatcher will apply an OR or AND operation to the terms to collate the composite result, which it then returns to the caller. 

Let's say we want to determine if a character is allowed to move.

There are typically numerous reasons why a character might not be allowed to move. They could be stunned, attacking, playing a special animation, dying, etc. All of these states would be subscribers to our movement allowed query.

For the purposes of this example, let's consider a frozen state, where a character can be frozen in place for a period of time. When the character is not frozen they can move freely and when frozen they remain stationary. We can model this with 2 behaviours: a CharacterMover and a Freezeable behaviour.

We will need to add a subscriber to our CharacterMover that returns a default answer of true. This represents the fact that, given no other modifiers, our character is allowed to move.

```csharp
using QueribusQuery;

public class CharacterMover : MonoBehaviour
{
  public static const string QUERY_CAN_MOVE = "Movement.CanMove";

  protected void OnEnable()
  {
    Queribus.SubscribeBool<GameObject>(QUERY_CAN_MOVE, GetCanMove);
  }
  
  protected void OnDisable()
  {
    Queribus.UnsubscribeBool<GameObject>(QUERY_CAN_MOVE, GetCanMove);
  }
  
  protected BoolResult GetCanMove(GameObject subject)
  {
    return new BoolResult(subject == gameObject, true);
  }
  
  // Rest of movement code would follow...
}
```
The `SubscribeBool()` subscriber needs a string identifier - a unique name for the query. We use a constant string `QUERY_CAN_MOVE` for compile-time safety and code navigability. The subscriber also needs the handler function that will be called to provide a result. For boolean queries such a handler must return a `BoolResult` object.

In our handler function `GetCanMove()`, we receive a parameter of the type we defined in our subscription call (in the example this is a `GameObject`). The value passed is defined by the dispatch call and can represent whatever you need it to represent. Typically it will be a GameObject that represents the object that the query is about. A suggested convention is to call such a GameObject the **subject** of the query.

The `BoolResult()` constructor takes 2 boolean arguments. The first is the **applicability** test. This represents whether the boolean result is relevant or applicable to the given subject. Testing if the subject is equal to the behaviour's `gameObject` (as in this example) is typical.

The second argument is the actual result we want to supply to the query if it is applicable. In this example we always want the character to be able to move if there is no other reason stopping them, so we return `true`.

Next we will need to add a subscriber to our Freezeable behaviour that returns false if the character is frozen.

```csharp
using QueribusQuery;

public class Freezeable : MonoBehaviour
{
  protected bool _isFrozen;

  protected void OnEnable()
  {
    Queribus.SubscribeBool<GameObject>(CharacterMover.QUERY_CAN_MOVE, GetCanMove);
  }
  
  protected void OnDisable()
  {
    Queribus.UnsubscribeBool<GameObject>(CharacterMover.QUERY_CAN_MOVE, GetCanMove);
  }
  
  protected BoolResult GetCanMove(GameObject subject)
  {
    return new BoolResult(subject == gameObject, !_isFrozen);
  }
}
```

Finally, when we want to know if a character can move, we can dispatch the query.

```csharp
if(Queribus.And(CharacterMover.QUERY_CAN_MOVE, gameObject)) 
{
  // Character can move
}
else 
{
  // Character cannot move
}
```

We supply our query identifier `CharacterMove.QUERY_CAN_MOVE` as well as our subject, the current `gameObject`. The `AND` dispatch takes all of the boolean results from our subscribers and ANDs them together. This means all of our subscribers need to return `true` in order for the query as a whole to return `true`. Consequently if there is at least 1 reason why our character cannot move, then our character cannot move.

Some syntactic sugar is provided for querying GameObject subjects - we can call a query as a member function of the GameObject in question:

```csharp
if(gameObject.QueryBoolAnd(CharacterMover.QUERY_CAN_MOVE)) 
{
  // Character can move
}
else 
{
  // Character cannot move
}
```

Both methods are functionally equivalent and simply represent different readability preferences.

## Summing a float
Queribus can sum together numerical values returned by dispatchers.

Let's say we want to compute how fast a character should move. 

First we need to set up our subscribers. We can set up a subscriber on the character that returns their default speed.

```csharp
using QueribusQuery;

public class CharacterMover : MonoBehaviour
{
  public static const string QUERY_MOVE_SPEED = "Movement.Speed";

  protected void OnEnable()
  {
    Queribus.SubscribeNumber<float, GameObject>(QUERY_MOVE_SPEED, GetDefaultMoveSpeed);
  }
  
  protected void OnDisable()
  {
    Queribus.UnsubscribeNumber<float, GameObject>(QUERY_MOVE_SPEED, GetDefaultMoveSpeed);
  }
  
  protected float GetDefaultMoveSpeed(GameObject subject)
  {
    return subject == gameObject ? 1f : 0f;
  }
  
  // Rest of movement code would follow...
}
```
In contrast to the `SubscribeBool()` subscriber the `SubscribeNumber()` subscriber takes 2 types: a number type and a subject type. The number type must be either `float` or `int`. As with other subscribers, our subject type can be whatever we need it to be.

The handlers for number queries must return a number of the appropriate type. In this example, since we subscribed to a float-typed query, our handler returns a `float`. In our handler we perform an applicability test by evaluating the subject against the behaviour's `gameObject`. If the result is applicable, we return it (in this case our default move speed of 1), otherwise we return 0, which will have no effect on the summing process.

Now say it's possible for our character to be chilled by some kind of cold effect, and that effect will slow their movement. We can define a subscriber in the cold effect that returns a negative value, which will lower the total speed result.

```csharp
using QueribusQuery;

public class Freezeable : MonoBehaviour
{
  protected float _chillAmount = 0f;

  protected void OnEnable()
  {
    Queribus.SubscribeNumber<float, GameObject>(CharacterMover.QUERY_MOVE_SPEED, GetMoveSpeed);
  }
  
  protected void OnDisable()
  {
    Queribus.UnsubscribeNumber<float, GameObject>(CharacterMover.QUERY_MOVE_SPEED, GetMoveSpeed);
  }
  
  protected float GetMoveSpeed(GameObject subject)
  {
    return subject == gameObject ? -_chillAmount : 0f;
  }
}
```

Finally we need to dispatch the query in our character movement code and set the character's speed to the result.

```csharp
public class CharacterMover : MonoBehaviour
{
  [SerializeField]
  protected float _minMoveSpeed = 0.25f;
  
  [SerializeField]
  protected float _maxMoveSpeed = 3f;

  protected _currentMoveSpeed;
  
  protected void FixedUpdate()
  {
    _currentMoveSpeed = Mathf.Clamp(gameObject.QueryFloatSum(QUERY_MOVE_SPEED), _minMoveSpeed, _maxMoveSpeed);
    
    // Rest of movement code would follow
  }
}
```
When we dispatch our `QUERY_MOVE_SPEED` query, all of the subscriber results will be summed together. This means we will gain our default move speed of 1 from the CharacterMover, then some negative value (or 0 if they are not chilled at all) from the Freezeable behaviour. The resulting sum will be equal to or less than 1, resulting in our character being slowed. We clamp the result to prevent excessively slow, fast or negative movement speeds.

## Calling a vote
Queribus is able to call a vote on what value some data type should have. Each subscriber will return their proposed value, along with a "vote weight", which represents how strongly the value should be considered by the dispatcher.

The value with the highest total weight is selected and returned to the caller.

Let's say we want to tint a character's colour based on whether they are frozen, poisoned or normal.

We can call a vote on what colour to set the character.

# License
MIT
