import { Blockchain, Transaction } from './blockchain.js';

const EC = require('elliptic').ec;
const ec = new EC('secp256k1');


const myKey = 
    ec.keyFromPrivate('be1a53f667596d0a533f6adbaacd9b1321672fd733cd0fe45bce7577332b946f');
const myWalletAddress : string = myKey.getPublic('hex');

console.log("My wallet address is: " + myWalletAddress);


let savjeeCoin = new Blockchain();

const tx1 = new Transaction(myWalletAddress, 'public key goes here', 10);
tx1.signTransaction(myKey);
savjeeCoin.addPendingTransactionToChain(tx1);

//savjeeCoin.addPendingTransactionToChain(new Transaction('address1', 'address2', 100));
//savjeeCoin.addPendingTransactionToChain(new Transaction('address2', 'address1', 50));

console.log('\n Starting the miner...');
savjeeCoin.minePendingTransactions(myWalletAddress);
console.log('\nBalance of xaviers is', savjeeCoin.getBalanceOfAddress(myWalletAddress));

// console.log('\n Starting the miner...');
// savjeeCoin.minePendingTransactions('xaviers-address');
// console.log('\nBalance of xaviers is', savjeeCoin.getBalanceOfAddress('xaviers-address'));
// console.log("Mining block 1...");
// savjeeCoin.addBlock(new Block(1, "10/01/2017", { amount: 4 }));
// console.log("Mining block 2...");
// savjeeCoin.addBlock(new Block(2, "11/01/2017", { amount: 10 }));

