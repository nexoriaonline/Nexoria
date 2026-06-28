# Manifesto do Projeto: Nexoria Online

## 1. Visão Geral do Projeto

**Nexoria Online** é um projeto ambicioso que visa criar um jogo de RPG online massivo (MMORPG) para a plataforma Android, inspirado no clássico Tibia (versão 8.60), mas com uma experiência de usuário otimizada para dispositivos móveis. O jogo incorporará controles intuitivos via joystick touch na tela e será construído sobre uma base técnica robusta e flexível, permitindo futuras customizações e a evolução contínua do mundo do jogo.

## 2. Componentes Técnicos Fundamentais

### 2.1. Servidor (Backend): The Forgotten Server (TFS) 1.5 Downgrade 8.60

O coração do nosso mundo virtual será o **The Forgotten Server (TFS) 1.5 Downgrade 8.60**, uma versão estável e altamente customizável do popular emulador de servidor de Tibia. Esta escolha estratégica nos permite herdar uma vasta gama de funcionalidades de jogo já implementadas e testadas, focando nossos esforços na adaptação e inovação.

-   **Repositório Base:** [nekiro/TFS-1.5-Downgrades (Branch 8.60)](https://github.com/nekiro/TFS-1.5-Downgrades/tree/8.60) [1]
-   **Linguagens:** C++ (core do servidor para performance) e Lua (para scripts de jogo, permitindo fácil modificação de magias, monstros, quests e sistemas).
-   **Banco de Dados:** MySQL ou TiDB, utilizando o `schema.sql` fornecido para gerenciar dados persistentes do jogo, como contas de jogadores, personagens, itens, casas e estados do mundo.
-   **Ambiente de Execução:** Preferencialmente Linux (Ubuntu) para estabilidade e performance, com compilação via CMake.

### 2.2. Cliente (Frontend): OTClientV8 para Android

Para a experiência do jogador em dispositivos móveis, utilizaremos o **OTClientV8**, um cliente alternativo de Tibia conhecido por sua otimização e capacidade multiplataforma. Sua arquitetura modular e suporte a scripts Lua o tornam ideal para a adaptação Android.

-   **Repositório Base:** [OTCv8/otclientv8](https://github.com/OTCv8/otclientv8) [2]
-   **Tecnologias:** C++ (engine gráfica com OpenGL ES 2.0 para compatibilidade móvel) e Lua (para a interface do usuário e lógica específica do cliente).
-   **Suporte Mobile:** O módulo `client_mobile` [3] do OTClientV8 é crucial, pois já contém a base para interações touch e adaptações de UI.
-   **Controles Touch:** A implementação do joystick virtual será baseada na lógica existente em `modules/client_mobile/mobile.lua` [4], que traduz toques na tela em movimentos direcionais do personagem. Isso será refinado para uma experiência de jogo fluida e responsiva.
-   **Interface de Usuário (UI):** A UI será customizada através de arquivos `.otui` e scripts Lua, permitindo a criação de botões de ação, inventário e chat otimizados para telas sensíveis ao toque.
-   **Compilação Android:** O cliente será compilado para Android (.apk) utilizando o Android NDK e SDK, seguindo guias de compilação como os encontrados na comunidade OTLand [5].

## 3. Filosofia de Desenvolvimento

Nossa abordagem será iterativa e focada na experiência do usuário mobile. Começaremos com a base funcional do Tibia 8.60 e, a partir daí, introduziremos melhorias e novas funcionalidades, sempre com a premissa de que o jogo deve ser divertido e acessível em smartphones e tablets.

-   **Reutilização:** Máximo aproveitamento da lógica existente do TFS e OTClientV8.
-   **Otimização Mobile:** Prioridade na performance e usabilidade em dispositivos Android, considerando diferentes tamanhos de tela e capacidades de hardware.
-   **Customização:** Liberdade para alterar gráficos, sons, sistemas de jogo e adicionar conteúdo exclusivo ao Nexoria Online.
-   **Comunidade:** Manteremos uma abordagem aberta para feedback e sugestões, evoluindo o jogo em conjunto com a comunidade de jogadores.

## 4. Próximos Passos Imediatos

1.  **Configuração do Ambiente de Desenvolvimento:** Estabelecer um ambiente funcional para o servidor TFS e para a compilação do cliente OTClientV8 para Android.
2.  **Validação da Base:** Garantir que o servidor e o cliente se comunicam corretamente e que o jogo base é jogável em um dispositivo Android com controles touch básicos.
3.  **Refinamento do Joystick:** Ajustar a sensibilidade e o layout do joystick virtual para uma jogabilidade ideal.

## 5. Referências

[1] nekiro/TFS-1.5-Downgrades (Branch 8.60). GitHub. Disponível em: [https://github.com/nekiro/TFS-1.5-Downgrades/tree/8.60](https://github.com/nekiro/TFS-1.5-Downgrades/tree/8.60)
[2] OTCv8/otclientv8. GitHub. Disponível em: [https://github.com/OTCv8/otclientv8](https://github.com/OTCv8/otclientv8)
[3] otclientv8/modules/client_mobile. GitHub. Disponível em: [https://github.com/OTCv8/otclientv8/tree/master/modules/client_mobile](https://github.com/OTCv8/otclientv8/tree/master/modules/client_mobile)
[4] otclientv8/modules/client_mobile/mobile.lua. GitHub. Disponível em: [https://github.com/OTCv8/otclientv8/blob/master/modules/client_mobile/mobile.lua](https://github.com/OTCv8/otclientv8/blob/master/modules/client_mobile/mobile.lua)
[5] Compiling OTCv8 for Android. OTLand. Disponível em: [https://otland.net/threads/compiling-otcv8-for-android.285125/](https://otland.net/threads/compiling-otcv8-for-android.285125/)
