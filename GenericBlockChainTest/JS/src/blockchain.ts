const SHA256 = require('crypto-js/sha256');
const EC = require('elliptic').ec;
const ec = new EC('secp256k1');

class Transaction{
    fromAddress: string | null;
    toAddress: string;
    amount: number;
    signature: any;
    constructor(fromAddress: string | null, toAddress: string, amount: number){
        this.fromAddress = fromAddress;
        this.toAddress = toAddress;
        this.amount = amount;
    }

    calculateHash(): string {
        return SHA256(this.fromAddress + this.toAddress + this.amount).toString();
    }

    signTransaction(signingKeyPair: typeof EC.KeyPair): void {
        if (this.fromAddress !== signingKeyPair.getPublic('hex')) {
            throw new Error('You cannot sign transactions for other wallets!');
        }
        
        const txHash = this.calculateHash();
        const sig = signingKeyPair.sign(txHash, 'base64');
        this.signature = sig.toDER('hex');
    }

    isValid(): boolean {
        if (this.fromAddress === null) return true;

        if (!this.signature || this.signature.length === 0) {
            throw new Error('No signature in this transaction');
        }

        const publicKey = ec.keyFromPublic(this.fromAddress, 'hex');
        return publicKey.verify(this.calculateHash(), this.signature);
    }
}

class Block
{
    timestamp: string;
    transactions: any;
    previousHash: string;
    hash: string;
    nonce: number = 0;
    constructor(timestamp: string, transactions: any, previousHash: string = ''){
        this.timestamp = timestamp;
        this.transactions = transactions;
        this.previousHash = previousHash;

        this.hash = this.calculateHash();
    }

    calculateHash(): string {
        return SHA256(this.previousHash + this.timestamp + JSON.stringify(this.transactions) + this.nonce).toString();
    }

    mineBlock(difficulty: number): void {
        while(this.hash.substring(0, difficulty) !== 
                    Array(difficulty + 1).join("0")){
            this.nonce++;
            this.hash = this.calculateHash();
        }

        console.log("BLOCK MINED: " + this.hash);
    }

    allTransactionsValid(): boolean {
        for (const tx of this.transactions) {
            if (!tx.isValid()) {
                return false;
            }
        }
        return true;
    }
}

class Blockchain{
    chain: Block[]; 
    difficulty: number;
    pendingTransactions: Transaction[];
    miningReward: number;
    
    constructor()
    {
        this.chain = [this.createGenesisBlock()];
        this.difficulty = 2;
        this.pendingTransactions = []; 
        this.miningReward = 100; 
    }
    createGenesisBlock(): Block {
        return new Block("01/01/2017", "Genesis Block", "0");
    }

    getLatestBlock(): Block {
        return this.chain[this.chain.length - 1];
    }

    // addBlock(newBlock: Block): void {
    //     newBlock.previousHash = this.getLatestBlock().hash;
    //     newBlock.mineBlock(this.difficulty);
    //     this.chain.push(newBlock);
    // }

    minePendingTransactions(miningRewardAddress: string): void {
        let rewardTx = new Transaction(null, miningRewardAddress, this.miningReward);
        this.pendingTransactions.push(rewardTx);

        let block = new Block(Date.now().toString(), this.pendingTransactions);
        block.mineBlock(this.difficulty);

        console.log('Block successfully mined!');

        this.chain.push(block);

        // Reward the miner
        this.pendingTransactions = [];
    }

    addPendingTransactionToChain(transaction: Transaction): void {
        if (!transaction.fromAddress || !transaction.toAddress) {
            throw new Error('Transaction must include from and to address');
        }

        if (!transaction.isValid()) {
            throw new Error('Cannot add INVALID transaction to chain');
        }

        this.pendingTransactions.push(transaction);
    }
    getBalanceOfAddress(address: string): number {
        let balance = 0;
        for (const block of this.chain) {
            for (const tx of block.transactions) {
                if (tx.fromAddress === address) {
                    balance -= tx.amount;
                }
                if (tx.toAddress === address) {
                    balance += tx.amount;
                }
            }
        }
        return balance;
    }

    isChainValid(): boolean {
        for(let i = 1; i < this.chain.length; i++)
        {
            const currentBlock = this.chain[i];
            const previousBlock = this.chain[i - 1];

            if (!currentBlock.allTransactionsValid()) {
                return false;
            }

            if (currentBlock.hash !== currentBlock.calculateHash()) {
                return false;
            }

            if (currentBlock.previousHash !== previousBlock.hash) {
                return false;
            }
        }
        return true;
    }
}

export { Blockchain, Transaction };