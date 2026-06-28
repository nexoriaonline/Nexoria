# Documento de Arquitetura e Roadmap: Jogo Tibia para Android

## 1. Introdução
Este documento apresenta uma proposta de arquitetura e um roadmap inicial para o desenvolvimento de um jogo estilo Tibia para a plataforma Android, com foco na versão 8.60 do Tibia e implementação de controles touch, incluindo um joystick virtual. A base do projeto será construída a partir de um servidor open-source existente e um cliente alternativo, ambos com comunidades ativas e recursos que facilitam a adaptação para dispositivos móveis.

## 2. Arquitetura do Servidor (Backend)

O servidor do jogo será fundamentado no **The Forgotten Server (TFS) 1.5 Downgrade 8.60**, mantido por Nekiro [1]. Esta escolha oferece uma base robusta e estável, com a lógica de jogo do Tibia 8.60, que é amplamente conhecida e apreciada pela comunidade.

### 2.1. Detalhes Técnicos
- **Repositório Base:** [nekiro/TFS-1.5-Downgrades (Branch 8.60)](https://github.com/nekiro/TFS-1.5-Downgrades/tree/8.60)
- **Linguagens:** O core do servidor é desenvolvido em C++, garantindo alta performance e eficiência. A lógica de jogo, como magias, monstros e sistemas de quests, é implementada em scripts Lua, o que permite uma customização flexível e relativamente fácil.
- **Banco de Dados:** Utiliza MySQL ou TiDB para armazenamento de dados de jogadores, itens, mapas e outras informações persistentes do jogo. O esquema do banco de dados (`schema.sql`) é fornecido no repositório, facilitando a configuração inicial.
- **Vantagens:** A principal vantagem de usar esta versão do TFS é a sua maturidade e a vasta quantidade de conteúdo e sistemas já implementados, permitindo que o foco do desenvolvimento seja na adaptação para mobile e na criação de novas funcionalidades, em vez de construir o backend do zero.

## 3. Arquitetura do Cliente (Frontend Android)

Para o cliente Android, a escolha recai sobre o **OTClientV8**, um cliente alternativo de Tibia altamente otimizado e multiplataforma [2]. O OTClientV8 é conhecido por sua flexibilidade e capacidade de ser estendido via scripts Lua, o que é crucial para a adaptação mobile.

### 3.1. Detalhes Técnicos
- **Repositório Base:** [OTCv8/otclientv8](https://github.com/OTCv8/otclientv8)
- **Tecnologias:** A engine é construída em C++, utilizando OpenGL ES 2.0 para renderização gráfica, o que a torna adequada para dispositivos móveis. A interface do usuário e grande parte da lógica do cliente são desenvolvidas em Lua, permitindo a criação de interfaces dinâmicas e personalizadas.
- **Suporte Mobile:** O OTClientV8 já inclui um módulo dedicado para dispositivos móveis, `client_mobile` [3]. Este módulo contém a lógica essencial para interações touch e adaptações de interface para telas menores.
- **Joystick Touch:** A funcionalidade de joystick virtual é implementada no arquivo `modules/client_mobile/mobile.lua` [4]. Este script Lua gerencia a conversão de toques na tela em comandos de movimento do personagem (Norte, Sul, Leste, Oeste e diagonais), simulando um joystick tradicional. Isso será a base para o controle de movimento do jogador.
- **Interface de Usuário (UI):** A interface é definida por arquivos `.otui` e scripts Lua. Isso significa que a customização de botões de ação, inventário, chat e outros elementos da UI para uma experiência touch otimizada será feita através da modificação desses arquivos.

## 4. Tecnologias Envolvidas

A tabela a seguir resume as principais tecnologias que serão utilizadas no projeto:

| Componente | Tecnologia Principal | Linguagens | Observações |
|:---|:---|:---|:---|
| **Servidor (Backend)** | The Forgotten Server (TFS) 1.5 | C++, Lua | Lógica de jogo, gerenciamento de mundo e jogadores. |
| **Banco de Dados** | MySQL / TiDB | SQL | Armazenamento persistente de dados do jogo. |
| **Cliente (Frontend)** | OTClientV8 | C++, Lua | Renderização gráfica, interface do usuário, lógica do cliente. |
| **Plataforma Mobile** | Android NDK/SDK | C++, Java (para integração nativa) | Compilação do cliente para APK, acesso a recursos do sistema Android. |

## 5. Roadmap Sugerido

Para iniciar e progredir com o projeto, os seguintes passos são recomendados:

1.  **Configuração do Ambiente de Desenvolvimento do Servidor:**
    *   Clonar o repositório `nekiro/TFS-1.5-Downgrades`.
    *   Instalar as dependências necessárias (compilador C++, CMake, MySQL/MariaDB/TiDB).
    *   Compilar o servidor e configurar o banco de dados inicial (`schema.sql`).
    *   Realizar testes básicos para garantir que o servidor está funcionando corretamente.

2.  **Configuração do Ambiente de Desenvolvimento Android para o Cliente:**
    *   Instalar o Android Studio, Android SDK e Android NDK.
    *   Clonar o repositório `OTCv8/otclientv8`.
    *   Configurar o ambiente de compilação para Android, seguindo as instruções da wiki do OTClientV8 (se disponíveis ou através de pesquisa) [5].
    *   Compilar o cliente para gerar um arquivo `.apk` e testar em um emulador ou dispositivo Android.

3.  **Análise e Customização do Joystick Touch:**
    *   Estudar a fundo o `mobile.lua` e `mobile.otui` dentro do módulo `client_mobile` do OTClientV8.
    *   Experimentar com diferentes layouts e sensibilidades para o joystick virtual.
    *   Considerar a adição de feedback tátil (vibração) ao usar o joystick.

4.  **Adaptação da Interface do Usuário (UI) para Mobile:**
    *   Redesenhar elementos da UI (barras de vida/mana, inventário, janelas de chat, botões de magia) para serem amigáveis ao toque e adequados para telas menores.
    *   Implementar botões de atalho rápido para magias e itens, posicionados de forma ergonômica.

5.  **Implementação de Funcionalidades Específicas para Mobile:**
    *   Desenvolver um sistema de 
auto-loot facilitado, sistema de mira assistida para combates, e outras melhorias de usabilidade.

6.  **Otimização de Performance:**
    *   Monitorar o desempenho do cliente em diferentes dispositivos Android.
    *   Otimizar o uso de recursos (CPU, GPU, memória) para garantir uma experiência de jogo fluida.

7.  **Testes e Iteração:**
    *   Realizar testes extensivos em diferentes tamanhos de tela e versões do Android.
    *   Coletar feedback dos usuários para refinar a jogabilidade e a interface.

## 6. Conclusão

Este projeto propõe uma abordagem sólida para o desenvolvimento de um jogo estilo Tibia para Android, aproveitando as bases existentes do TFS 8.60 e do OTClientV8. A combinação dessas tecnologias, juntamente com um foco na experiência do usuário mobile (controles touch e joystick virtual), oferece um ponto de partida promissor para criar um jogo envolvente e customizável. O roadmap detalhado acima servirá como guia para as próximas etapas de desenvolvimento, permitindo uma progressão estruturada e eficiente.

## 7. Referências

[1] nekiro/TFS-1.5-Downgrades (Branch 8.60). GitHub. Disponível em: [https://github.com/nekiro/TFS-1.5-Downgrades/tree/8.60](https://github.com/nekiro/TFS-1.5-Downgrades/tree/8.60)
[2] OTCv8/otclientv8. GitHub. Disponível em: [https://github.com/OTCv8/otclientv8](https://github.com/OTCv8/otclientv8)
[3] otclientv8/modules/client_mobile. GitHub. Disponível em: [https://github.com/OTCv8/otclientv8/tree/master/modules/client_mobile](https://github.com/OTCv8/otclientv8/tree/master/modules/client_mobile)
[4] otclientv8/modules/client_mobile/mobile.lua. GitHub. Disponível em: [https://github.com/OTCv8/otclientv8/blob/master/modules/client_mobile/mobile.lua](https://github.com/OTCv8/otclientv8/blob/master/modules/client_mobile/mobile.lua)
[5] Compiling OTCv8 for Android. OTLand. Disponível em: [https://otland.net/threads/compiling-otcv8-for-android.285125/](https://otland.net/threads/compiling-otcv8-for-android.285125/)
