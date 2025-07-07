# Re:memoria
![image](https://github.com/user-attachments/assets/0c8964f8-721a-4b91-a135-2b794a5c2dc2)

<p align="center">
  <img src="https://github.com/user-attachments/assets/07e36dee-4316-4046-aa9e-91084795fe47" alt="memory2">
</p>

## Idea
Simple memory game with code driving the randomizing of cards.

## Backstory
The cowboy is talking to someone, and he needs to pair items to communicate with them.

## To Do
In the original game, you flip all the cards first, then make pairs. In this version, every time you match two cards, a lamp in the thought-cloud lights up and backlights the icons. Also add a tornado effect that shuffles the cards inside the cloud.

- [ ] Create a menu to start, a pop up when game is over and a menu that you can access whenever.
- [ ] Add some chill guitar music.
- [ ] Create highscore based on time or number of flips.

The cards are created in code to allow different number of cards.
Added some animations of the background to make it more interesting.


# Development

## Obstacles along the way

I am using URP at the moment with a fullscren effect. I wanted to add a simple haze effect but I realized that all opaque object are the only ones getting the effect, not the transparent objects. Only a problem with the tumble weed effect. I do not at this time know if this will affect anything in the game that I want to add later, because I might want other full screen effect (rain etc) I will probably have to bite the apple and just fix the order of the shader so it is applied AFTER transparent objects are drawn. I added a script that takes the RGB after transparency. I do not think it was necessary. I managed to realize that the material I needed was unlit/transparent cutout for the "transparent" sprites and it works. I will backtrack and see if I can remove the material script.



Thought I could create a simple double-sided shader for turning the cards. 
I was thinking of creating a box, but settled with 2 facing sprites and letting them rotate around a pivot point.


![Memory cards](https://github.com/user-attachments/assets/677f7a85-971a-4b5e-971a-3eea0d492f7f)

Animationclip is legacy, so I needed to find current documentation to make this work. I could have flipped the cards using code but I would rather use the engine for that.
