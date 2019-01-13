# Xiropt-Wallet

This is the official Xiropht Wallet Gui, this one is compatible with Netframework 4.6 and Mono. 

**In production, we suggest to compile the wallet in Release Mode for disable log files.**


Windows:

- For compile the project Visual Studio or Mono is required.

Linux:

- For compile the project Mono is required, you can also compile the project with Visual Studio and make a Linux Binary.

For make a binary linux file from executable windows file:

mkbundle Xiropht-Wallet.exe -o Xiropht-Wallet Xiropht-Connector-All.dll MetroFramework.Fonts.dll MetroFramework.dll MetroFramework.Design.dll --deps -z --static


Informations:

- Xiropht wallet gui provide the possibility to get your current balance without to be sync at 100%.

- You can send/receive transaction without to be sync at 100%.

- The wallet gui will always sync accurate transaction informations, he will never ask the whole transaction data of the network, only yours.

- Some options of sync, can be help you to choose the right one, you can use by default seed nodes for sync your wallet or you can use the public list of remote node and seed nodes together. You can also use your own private remote node , this option is only recommended once you select your own remote node. 

- The wallet gui will always contact seed nodes for check every informations provided by remote nodes listed on the public list of them before to use them.

- The pin code asked by the blockchain can be disabled, this option is independent for each wallet.

- Remember to save somewhere your wallet informations: private key, public key, pin code just in case.

- The xiropht network don't allow multiple connections on the same wallet. 

- The xiropht network provide to your wallet gui an approximative time of receive when you try to send a transaction.

- Every information what you get on your transaction history cannot be read by another wallet

For more informations about how work the network connection of wallet, please check the WhitePaper of Xiropht.

**Be sure to compile in release mode those source for don't enable Log System of the wallet**

**Xiropht-Connector-All Library is required for compile the gui wallet: https://github.com/XIROPHT/Xiropht-Connector-All**

**Language files contributors:**

- English: Xiropht, DigitalTwister, AlpHA

- French: Xiropht

- Greek: Alpha

- Hungarian: Maxy86

**Official testers:**

- DigitalTwister
- Sniperviperman
- AlpHA
- Maxy86
- Wolfierawr
- Rashed
- Sabrar
- Xiropht
