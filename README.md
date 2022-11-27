# **Artifact Of Revolution**

Artifact of Revolution is a slight alteration to the existing artifact.

## **Normal Config**
The main alteration is how many of an item is given to the monsters, by default its roughly half the current stage plus or minus the tier of the item. In the this default setting the first loop through should feel relatively unaltered by the mod, after the first loop you should notice an increase in the amount of an item they receive.

## **Hard Config**
Hard mode is a minor alteration of normal by making the ramp up of items on the enemy team faster.
accessing this is simply by adjusting your config file for Artifact Of Revolution to say `"Hard"`

## **Chaos Config**
Chaos is full on 1:1 between current stage and items given to the enemies. this mode will be extremely chaotic due to [how the evolution artifact works](#how-evolution-grants-tiered-items). just like hard to access this simply alter the config file to say `"Chaos"`


## **How Evolution Grants Tiered Items**
Unfortunately I have yet to look into how to alter the items tiers and when they are given (personally i would love to randomize this but im a noob). below is the order of tiers given to the Monsters every loop.

```diff
  Common item
  Common item
+ Uncommon item
+ Uncommon item
- Rare item
```
following the `Rare item` it will go back to common.


## changelog

**1.0.0**

* Release of the mod.