# Base do Projeto: Tibia Android (Estilo 8.60)

## 1. Servidor (Back-end)
O servidor será baseado no **TFS 1.5 Downgrade 8.60** da Nekiro.
- **Repositório:** [nekiro/TFS-1.5-Downgrades (Branch 8.60)](https://github.com/nekiro/TFS-1.5-Downgrades/tree/8.60)
- **Linguagem:** C++ (Core) e Lua (Scripts de jogo).
- **Banco de Dados:** MySQL/TiDB (usando o `schema.sql` fornecido).
- **Vantagem:** Estabilidade do TFS 1.5 com o protocolo clássico 8.60, permitindo alta customização.

## 2. Cliente (Front-end Android)
Utilizaremos o **OTClientV8** como base para o cliente Android.
- **Repositório:** [OTCv8/otclientv8](https://github.com/OTCv8/otclientv8)
- **Tecnologia:** C++ (Engine) e Lua (Interface e lógica de jogo).
- **Suporte Mobile:** Já possui um módulo nativo chamado `client_mobile`.
- **Joystick Touch:** A lógica de joystick já existe no arquivo `modules/client_mobile/mobile.lua`, utilizando as funções `onKeypadTouchPress`, `onKeypadTouchMove` e `executeWalk` para converter toques na tela em movimentos do personagem (N, S, E, W, NW, NE, SW, SE).

## 3. Arquitetura Proposta
- **Server-Side:** Hospedagem em Linux (Ubuntu) com compilação via CMake.
- **Client-Side:** Compilação do OTClientV8 para Android (.apk) usando NDK e SDK do Android.
- **Interface:** Customização via arquivos `.otui` e `.lua` no cliente para adaptar botões de magias, inventário e chat para telas touch.

## 4. Próximos Passos (Plano de Ação)
1. **Configuração do Servidor:** Clonar e compilar o TFS 8.60 no ambiente de desenvolvimento.
2. **Ambiente Android:** Configurar o SDK/NDK para compilar o OTClientV8.
3. **Customização de Joystick:** Refinar a sensibilidade e o layout do joystick touch no `mobile.otui`.
4. **Iteração de Ideias:** Adicionar sistemas exclusivos para mobile (ex: botões de atalho rápido maiores, auto-loot facilitado).
