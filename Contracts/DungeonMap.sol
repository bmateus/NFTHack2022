// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "@openzeppelin/contracts/token/ERC721/ERC721.sol";
import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/utils/math/SafeMath.sol";
import "@openzeppelin/contracts/utils/Counters.sol";

//import "hardhat/console.sol";

contract DungeonMap is ERC721, Ownable {
  using SafeMath for uint256;
  using Counters for Counters.Counter;

  string public tokenBaseURI;

  bool public mintActive = false;

  Counters.Counter public tokenSupply;

  uint256 price = 0;

  mapping(address => )

  constructor() ERC721("DUNGEON MAP", "MAP") {
  }

  /************
   * Metadata *
   ************/

  function setTokenBaseURI(string memory _baseURI) external onlyOwner {
    tokenBaseURI = _baseURI;
  }

  function tokenURI(uint _tokenId) override public view returns (string memory) {
    return string(abi.encodePacked(tokenBaseURI, _tokenId));
  }

  /********
   * Mint *
   ********/

  function mint() external payable 
  {
    require(mintActive, "Sale is not active.");
    require(msg.value == price, "The value sent is not correct");
    uint mintIndex = tokenSupply.current();
    tokenSupply.increment();
    _safeMint(msg.sender, mintIndex);
  }


  /***********
   * Collect *
   ***********/

// This function allows the caller to burn this NFT in exchange for the resources
// requires a signature to verify that everything is legit and did not cheat
// The dungeon loot is placed into the provided player object
   function collectAndBurn(uint256 playerTokenId, uint256 mapId, 
    uint256[] lootTypes, uint256[] lootAmounts, bytes calldata _signature) external 
   {
    //verify
   	bytes data = abi.encode(playerTokenId, mapId);
   	for(int i=0; i < lootTypes.length; i++)
   	{
   		data = abi.encode(data, lootTypes[i], lootAmounts[i]);
   	}
   	require(verifyOwnerSignature(keccak256(data), _signature), "Invalid signature");

    //award loot
	for(int i=0; i<lootTypes.length; i++)
	{
		lootContract[lootTypes[i]].mintForPlayer(playerTokenId, lootAmounts[i]);
	}

    //burn map
	burnToken(mapId);

   }

   function verifyOwnerSignature(bytes32 hash, bytes memory signature) private view returns(bool) 
   {
      return hash.toEthSignedMessageHash().recover(signature) == owner();
    }


  /**************
   * Admin Only *
   **************/

  function setMintActive(bool _active) external onlyOwner {
    mintActive = _active;
  }

  function setPrice(uint256 newPrice) public onlyOwner {
  	price = newPrice;
  }

  function withdraw() public onlyOwner {
    uint balance = address(this).balance;
    payable(msg.sender).transfer(balance);
  }




}