// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "@openzeppelin/contracts/token/ERC721/ERC721.sol";
import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/utils/math/SafeMath.sol";
import "@openzeppelin/contracts/utils/Counters.sol";

//import "hardhat/console.sol";

contract DungeonPlayer is ERC721, Ownable {
  using SafeMath for uint256;
  using Counters for Counters.Counter;

  string public tokenBaseURI;

  bool public mintActive = false;

  Counters.Counter public tokenSupply;

  uint256 price = 0;

  constructor() ERC721("DUNGEON ADVENTURER", "ADV") {
  }

  /************
   * Metadata *
   ************/

  function setTokenBaseURI(string memory baseURI) external onlyOwner {
    tokenBaseURI = baseURI;
  }

  function _baseURI() internal view override returns (string memory) {
        return tokenBaseURI;
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