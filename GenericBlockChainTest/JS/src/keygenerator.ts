const EC = require('elliptic').ec;

const ec = new EC('secp256k1');

const keyPair : typeof EC.KeyPair = ec.genKeyPair();

const publicKey = keyPair.getPublic('hex');
const privateKey = keyPair.getPrivate('hex');

console.log("\nPrivate Key: " + privateKey);
console.log("\nPublic Key: " + publicKey);