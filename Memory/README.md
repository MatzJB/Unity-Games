# Re:memoria
![image](https://github.com/user-attachments/assets/0c8964f8-721a-4b91-a135-2b794a5c2dc2)


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

First version spring 2020:

<p align="center">
  <img src="https://github.com/user-attachments/assets/07e36dee-4316-4046-aa9e-91084795fe47" alt="memory2">
</p>

## Idea
Simple memory card game with some fun game mechanics.
The feeling should be that the cowboy character is a comic book character. 

This is mainly because he is not moving because I don't have any ability to animate. He is a sprite and at most I can add poses, just like a comic book. Continuing with that idea, maybe when he change position, we should see a page turn. This illusion is broken by the fact that the world is moving, is it only in his mind?

## Shader ideas

Note: Since the heat haze is rendered after transparents, a unlit transparent will not be affected by it. Everything that is opaque (the background) will.

<img width="458" alt="image" src="https://github.com/user-attachments/assets/6ded5044-cbd6-4462-a841-c910aecad405" />



### Heat haze 
I use "heat haze", which I think make sense, since the cowboy is in the desert after all. I couldn't find any source of information that took me all the way, but Youtube helped me some. I had to create this shader twice because Unity crashed hard once and this shader became corrupt.

<img width="500"  alt="image" src="https://github.com/user-attachments/assets/d9cd4421-6fd4-4ac7-83a3-e5c179d6d649" />


### Lighting up cards
The light buld bonus will light up the cards from behind when it is triggered. This is a simple effect inside my double sided shader triggered by code when the specific bonus is used. 


## Backstory
You are a cowboy walking in the desert to Mexico. One the way you think about food and how you should order it in Spanish.


<img width="500" alt="Eating" src="https://github.com/user-attachments/assets/876eb676-dd5f-41db-863b-28c01b1dda9c" />


## Game mechanics
In the original game, you flip all the cards first, then make pairs as you flip them over during the course of a game. In this version, every time you match two cards, you will get a bonus, that you can use in a later stage if you wish. For now there is only two bonuses. One that lights up the cards (idea lamp) and one that flips the cards (wind).

The cards show a word and an item used in restaurants in Spanish. To be able to match cards without knowing what they mean, I added a colored item to each cards.


## To Do

- [ ] Create a menu to start, a pop up when game is over and a menu that you can access whenever.
- [ ] Add some chill guitar music.
- [ ] Create highscore based on time or number of flips.

The cards are created in code to allow different number of cards.
Added some animations of the background to make it more interesting.


## Considerations while coding
I am not an expert Unity game dev, but I try to apply sane principles I developed from my dev carreer. First, I try to adhere to a Single Responsibility principle, and I try to go by the principle of making the code as clear, simple to understand and concise as possible. I generally avoid writing comments, the code and naming of functions and variables should be enough to understand what is going on.

I try to keep each script file less than 300 lines of code, if I cannot I seriously consider to either split it up or create functions that I put in a static "misc" class (which can be larger than 300 lines).

The GameState is a Singleton, this might be a poor "anti pattern" (according to Youtubers), but unless I do that with everything, I think it is fine. I felt the need to do it in this way because I really don't want to put everything on gameObjects if I can help it. I also use events when applicable. For this version I purposefully don't care about proper GUI strcuturing (placing gameobjects in canvas) and allowing for resizing canvas and letting the GUI follow. I put that in things to do later, but for now I don't care.

Disclosure: I use AI (chatGPT) to help me when I am at a loss. If I get stuck in architecture design considerations, I would ask if there is a better way and carefully consider all alternatives, but ultimately I am coding everything myself. 
For code that was solely generated by AI from a simple prompt, I put a comment in the code. All of the art is AI generated (the design took a long time to get right).

I want the content to be cached at runtime when I first run the game (deck and levels). Additionally, I want the menus to be in the current scene, in fact, I don't use scenes at all, just one scene that is drawn to, erased and redrawn into. To switch between scenes, I use a black quad that is a child of the camera and modify its opacity whenever I wish to fade to black, show text and fade back again. This was suggested to me by ChatGPT, I made the choice to add the blackout quad to my camera, so it always follows it. I need to make sure the text is in front of the quad, seen from the camera.


# Development

## Obstacles along the way and how I solved them

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

* I am using URP at the moment with a fullscren effect. I wanted to add a simple haze effect but I realized that in doing this I can only use opaque material and alpha cutting.
To avoid the FS shader to affect some parts I simply use the transparent materials on those item.
Edit: You can use transparent objects?

* When I first developed this game in 2020, I thought I could create a simple double-sided shader for turning the cards. This turned out to be diffuclt, so I ended up using two out-facing sprites and letting them rotate around a pivot point between them.

![Memory cards](https://github.com/user-attachments/assets/677f7a85-971a-4b5e-971a-3eea0d492f7f)

When I revisited the game in 2025, I realized, after I had gained some experience using Shader Graphs, I could create a double-sided shader instead. It not only make the game simpler, using only "half" as many geometries but I also needed it for my see-through effect (the idea light bulb).

* Animationclip is legacy, so I needed to find current documentation to make this work. I could have flipped the cards using code but I would rather use the engine for that.

* I don't know if this is good but to make life simple, the tumbleweed has two parents, one for translation and one for rotation. I use a abs(cos) to get it jumping and I also rotate it using the same idea, slightly phased so as to get the feel it touched ground and gains rotational momentum.

* When I first coded my game in 2020, I was relying on gameObjects a lot. I instantiated the cards and named them card_i_j, to pick them I parsed the name string each time.
  I knew this was hacky, and I learned that it is not necessary. In this newer version, while refactoring, I realized that the GameState should read a JSON of a deck, I should create cardObjects that holds Objects of "Card" and gameObject, so I could easily trigger things in the card object, instead of looking them up each time, which is not nice.

* cards were a matrix up until recently when I realized that I want to move around the cards and there were no real reason to name them anything special, since I keep a reference to the gameObject. So I use a List<Card> instead, which is simpler. This makes the randomizer code very simple and it delegates the responsability of gameObject layout to the "buildBoard" method within CreateCards.cs.
  
* At one point I changed the execution order to resolve issues I had with createCards.cs not executing before the GameState.cs. I have since then abandoned that idea, and now I make sure the GameState singleton runs first by simply instancing it in "awake". CreateCards.cs will be called in GameState.cs for each stage in the game, so it make more sense to do it in this way: starting the game by executing the same code as I do when I change stage.

* In the first version of the game everything about the cards where hard-coded and I had no consideration of making levels. As I revistited the game, I thought I would want to make levels but I wanted all that data to be accessed from the outisde, in a JSON. So, in this updated version I added a JSON for levels and one for the deck. The deck contains all information about the cards that is used in the game. I purposefully left out "state" of the cards, that is something the serializer do not need to worry about as it is not part of the raw information of the cards, but only the state of the game.

* I had some issues trying to apply the lighting of the double sided shader variable, but it turned out when I traversed the card gameObject I found the wrong parent that had a script attached to it and everything, but it turned out to be a disabled script. In my trying to get my code to work I added script to the wrong level.

# Issues
* Between stages, clicking a card before the cards have turned back over will break the animation state of the cards. Solution: prevent clicking of cards that are in the state of "rotating", maybe introduce another state in the CardObject class?

* Between stages, the cards will redraw and immediatately the stage is over and the next one starts. It would have been nice with a pause between stages and maybe a text stating what stage I am in. I've been thinking about the JSON file should maybe contain the background and if the background should be affected by the heat haze effect. Also, if tumbleweed can be shown (inside the restaurant it cannot).

* The tumble weed should only be visible when the bonus "wind" is used

* Use fading when introducing objects on the screen

* The bonuses should shine and "blink", create that shader

* The ratio and the background doesn't follow when you resize the editor

* Add audio for: card flipping, wind, ambient, light bulb, maybe each card could have him (in his mind) mumble the word in spanish
  
* Add blinking of the eyes of the cowboy


<img width="400"  alt="Wanted" src="https://github.com/user-attachments/assets/b45956e8-466d-4213-9caf-3fc0f7c5b636" />

  

