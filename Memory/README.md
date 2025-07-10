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

* I am using URP at the moment with a fullscren effect. I wanted to add a simple haze effect but I realized that in doing this I can only use opaque material and alpha cutting.
To avoid the FS shader to affect some parts I simply use the transparent materials on those item.


* In 2020 I thought I could create a simple double-sided shader for turning the cards. 
I was thinking of creating a box, but settled with 2 facing sprites and letting them rotate around a pivot point.

![Memory cards](https://github.com/user-attachments/assets/677f7a85-971a-4b5e-971a-3eea0d492f7f)

When I revisited the game in 2025 I realized I could create a double sided shader instead. This was necessary to creat the see-through effect and it made the scene simpler.

* Animationclip is legacy, so I needed to find current documentation to make this work. I could have flipped the cards using code but I would rather use the engine for that.

* When I first coded my game in 2020, I was relying on gameObjects a lot. I instantiated the cards and named them card_i_j, to pick them I parsed the name string each time.
  I knew this was hacky and I learned that it is not necessary. In the later version I refactored the code and put a reference of the gameObject inside the *cards* in code instead.

* cards were a matrix up until recently when I realized that I want to move around the cards and there were no real reason to name them anything special since I keep a reference to the gameObject. So I use a List<Cardc> instead, which is simpler.
  

  

