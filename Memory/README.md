# Re:memoria
![image](https://github.com/user-attachments/assets/0c8964f8-721a-4b91-a135-2b794a5c2dc2)


## Idea
The classic "memory" game.
It started as a mock up game to prove to my wife a game could be accomplished in a couple of hours, this was back in 2020. I recently picked it up again and thought I would improve the structure and playability.

## Backstory
You play as a cowboy crossing the desert toward Mexico, exhausted and starving. As the journey drags on, his thoughts take over.

These thoughts form a “thought cloud” where language appears as floating cards. Words, symbols, and fragments drift, collide, and reorganize. You experiment, learn patterns, and build meaning under pressure.

At the end, you reach a restaurant. You don’t type or speak Spanish. You assemble it.

Using the cards you’ve learned, you construct phrases to communicate with the waiter. Get it right, you eat. Get it wrong, the restaurant fades. It was just a mirage.

## Game mechanics
In the original game, you flip all the cards first, then make pairs. In this version, every time you match two cards, you will get a bonus to bail you out. There are two bonuses: a lamp lighting up, showing the other side of the cards or the wind, which will make the cards spin for a awhile.


## To Do

- [ ] Create a menu to start, a pop up when game is over and a menu that you can access whenever.
- [ ] Add some chill guitar music.
- [ ] Create highscore based on time or number of flips.
- [ ] Heat haze effect on the desert
- [ ] make the cards move and rotate a little, they ate floating after all
- [ ] When hovering the cards (with the mouse) they should increase in size and the opposite when not hovering
- [ ] when the game is done, show a splash screen of some sort
- [ ] start with 8 cards, then next game 12, then increase the number of cards
- [ ] add JSON with levels
- [ ] between games, introduce the new cards floating int the cloud and enter a new scene, the cowboy has walked quite a bit
- [ ] at the end, the arrives in mexico and he gets his reward, some food, stringing together what the cards showed previousl

# Development

First version spring 2020:

<p align="center">
  <img src="https://github.com/user-attachments/assets/07e36dee-4316-4046-aa9e-91084795fe47" alt="memory2">
</p>

Updated version (didn't touch the code in 5 years) 2025:


## Obstacles along the way

### Placing the cards
In my first version I placed the cards using a hard-coded position and I calculated the positions of the cards based on the monitor size. 
I am aware there is `Grid` but I just wanted to get started without extra obstacles. Well, this was one obstacle regardless.
https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Grid.html
So, I settled on a solution that gets the position of the cloud where the cards are supposed to be. I realized that the card size is not normalized, which made my head ache (why didn't I normalize the size of the card, makes life so much easier), so I finally normalized the card dimensions.
I calculate the width of the cloud, the origin and I just scale the Cards which all the cards are under and voila.
I instanciate the card object and I name it based on the index in the 2d array, so when I need to know what card I hit, I extract that information from the card name.
Note to self: I can add this metadata to the object without having to parse the string, right?


### Making the cards actually float
I wanted the cards look as though they really float. The actual card node has flip animation, so I couldn't add the rotation there, it will reset because it is keyframed. So my solution was to add a parent node to that node, I call it Card. 
```
**Before**

+ Cards
    + the_card_1_1 (holds animation)
         master_card
         master_card_back
    ...

**After**

+ Cards
    + Card_1_1 (holds initial rotation)
       + the_card_1_1 (holds animation)
            master_card
            master_card_back
    ...
```



### Procedurally generated cards
The cards were created in code to allow different number of cards. I now settle for 2 matchings, 3 is too difficult to manange for players.

* I started using hard coded cards and then switched to a JSON file to curate the cards. Everything is in the JSON: the groupIndex (same index means matching card), cardIndex (unique ID), the category (for levels, might be changed to level instead).

* Cloud shader

* Thought I could create a simple double-sided shader for turning the cards. 
I was thinking of creating a box, but settled with 2 facing sprites and letting them rotate around a pivot point.


![Memory cards](https://github.com/user-attachments/assets/677f7a85-971a-4b5e-971a-3eea0d492f7f)

Animationclip is legacy, so I needed to find current documentation to make this work. I could have flipped the cards using code but I would rather use the engine for that.
