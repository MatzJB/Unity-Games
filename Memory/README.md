# Re:memoria
![image](https://github.com/user-attachments/assets/0c8964f8-721a-4b91-a135-2b794a5c2dc2)

<p align="center">
  <img src="https://github.com/user-attachments/assets/07e36dee-4316-4046-aa9e-91084795fe47" alt="memory2">
</p>

## Idea
The classic "memory" game.
It started as a mock up game to prove to my wife a game could be accomplished in a couple of hours.

## Backstory
You are a cowboy walking in the desert toward Mexico and with nothing else to do he thinks about the spanish words he needs to remember when he arrives. In the game he stops to think and the thoughts are depicted using a cloud. I chose a cartoony "thought cloud" because I could not think of a way to show the desert and have the cards hanging in the air (it doesn't make sense). However, having the cards be a figment of his imagination does make sense, so they float in the cloud! Continuing on that theme, I thought, why add interesting physics to the cloud, so I added lightning. Then I thought, why not add a light bulb, that is the actual object you use when you tell the audience "I have an idea". In this setting the lamp will light up the cards from behind so you can see the symbols.



## To Do
In the original game, you flip all the cards first, then make pairs. In this version, every time you match two cards, a lamp in the thought-cloud lights up and backlights the cards. I am thinking of adding a tornado effect that shuffles the cards inside the cloud.

I also thought about matching 3 or 4 cards but that is probably too difficult.

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

## Obstacles along the way

### Placing the cards
In my first version I placed the cards using a hard-coded position and I calculated the positions of the cards based on the monitor size. This is always messy but I wasn't aware of Grid and it made it difficult to move around the cards where I wanted them to be. So, I used Grid instead, which solves all of the details for me.
https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Grid.html

### Procedurally generated cards
The cards were created in code to allow different number of cards. I now settle for 2 matchings, 3 is too difficult to manange for players.

* I started using hard coded cards and then switched to a JSON file to curate the cards. Everything is in the JSON: the groupIndex (same index means matching card), cardIndex (unique ID), the category (for levels, might be changed to level instead).

* Cloud shader

* Thought I could create a simple double-sided shader for turning the cards. 
I was thinking of creating a box, but settled with 2 facing sprites and letting them rotate around a pivot point.


![Memory cards](https://github.com/user-attachments/assets/677f7a85-971a-4b5e-971a-3eea0d492f7f)

Animationclip is legacy, so I needed to find current documentation to make this work. I could have flipped the cards using code but I would rather use the engine for that.
