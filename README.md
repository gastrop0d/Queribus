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

For the purposes of this example, let's consider a frozen state, where a character can be frozen in place for a period of time. We will need to add a subscriber to our frozen behaviour.

We will also need to add a subscriber to our character that returns a default answer of true. This represents the fact that, given no other modifiers, our character is allowed to move.

## Summing a float
Queribus can sum together numerical values returned by dispatchers.

Let's say we want to compute how fast a character should move. 

First we need to set up our subscribers. We can set up a subscriber on the character that returns their default speed.

Now say it's possible for our character to be chilled by some kind of cold effect, and that effect will slow their movement. We can define a subscriber in the cold effect that returns a negative value, which will lower the total speed result.

Finally we need to dispatch the query in our character movement code and set the character's speed to the result.

## Calling a vote
Queribus is able to call a vote on what value some data type should have. Each subscriber will return their proposed value, along with a "vote weight", which represent how strongly the value should be considered by the dispatcher.

The value with the highest total weighted is selected and returned to the caller.

Let's say we want to tint a character's colour based on whether they are frozen, poisoned or normal.

We can call a vote on what colour to set the character.

# License
MIT
