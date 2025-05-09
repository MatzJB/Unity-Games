# Unity-Games

## Memory

![Uploading memory2.gif…]()

Simple memory game with code driving the randomizing of cards.
The cards are created in code to allow different number of cards.
Added some animations of the background to make it more interesting.

## TODO
- [ ] Make sure all the cards are visible based on screen regardless the number of cards.

- [ ] Create a menu to start, a pop up when game is over and a menu that you can access whenever.

- [ ] Create highscore based on time or number of flips.

- [ ] Add some chill guitar music.


### Obstacles along the way

Thought I could create a simple double-sided shader for turning the cards. 
I was thining of creating a box, but settled with 2 facing sprites and letting them rotate around a pivot point.


![Memory cards](https://github.com/user-attachments/assets/677f7a85-971a-4b5e-971a-3eea0d492f7f)

Animationclip is legacy, so I needed to find current documentation to make this work. I could have flipped the cards using code but I would rather use the engine for that.
