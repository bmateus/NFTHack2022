# My NFTHack 2022 Project: "NFT Dungeon Adventure"

Mint an adventurer and a dungeon map and fight monsters and find treasure!

## What is is?

The idea of this project was to utilize the new Moralis Unity SDK to create a simple Blockchain-Enabled Unity game. The game would allow a player to mint an "adventurer" - a NFT on Polygon with procedural appearance and stats. This adventurer would represent the player's progress in the game.  After minting an adventurer, a player would then be able to mint a dungeon map. The dungeon map is a procedurally generated NFT that contains monsters and treasure. The player would then be able to use their adventurer to explore the map, fighting monsters and gaining treasures. The moralis server database is used to track the players progress. When the player is done, they can "burn" their map with a transaction that would place all the earned treasures into their adventurers "inventory". The player can then decide to use the treasure or break it down into materials to craft new equipment for their adventurer!

## Details

The Moralis Unity SDK is pretty new so I wanted to go through the process of building something with it to see what it was capable of and to discover if it had any bugs. I spent some time creating a startup scene in unity that would connect the users wallet with WalletConnect and login to the Moralis server. Next, I created a simple dungeon exploration game in Unity and figured out what kind of mechanics the game would need on-chain and off-chain. My plan was to have the Moralis server keep track of the player's progress in the dungeon - the player's health, what monsters they have slain and what treasure the player has collected. After that I created a contract for the Adventurer - I wanted to create a contract that could "hold" other Tokens but I don't have much experience with that and it would take longer than the available time. I wanted to show the ENS name of the player in game, but it seems there is a bug with the Moralis Unity SDK where this is not supported(I will file a bug about it!). After this I created a set of procedurally generated avatars using different layered images and uploaded the metadata and image data to IPFS using Pinata. Afterwards, I started working on the dungeon contract. A player would be able to burn their dungeon once they had completed it to gain the rewards. The Moralis server would provide a signature so that we could validate on-chain that the arguments to this function were not tampered with to prevent cheating. 

##Contracts
Deployed (and verified) on Polygon:

- DungeonPlayer.sol (You can use this to mint an adventurer / DUNGEON ADVENTURER / ADV)
https://polygonscan.com/address/0xF7eF54F931ccBdbad5e6Bf69badFD5af3C202848#code

