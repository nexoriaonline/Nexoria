# Prompt Mestre: Construção de Nexoria Online (Fase Inicial)

## Objetivo Geral

Construir a base funcional do jogo MMORPG **Nexoria Online** para a plataforma Android, utilizando o The Forgotten Server (TFS) 1.5 Downgrade 8.60 como backend e o OTClientV8 como frontend, com foco na implementação e refinamento dos controles de joystick touch na tela e adaptações iniciais da interface do usuário para mobile.

## Contexto e Referências

Este projeto é guiado pelo documento **"Manifesto do Projeto: Nexoria Online"** (`/home/ubuntu/nexoria_online_manifesto.md`), que detalha a visão, arquitetura e tecnologias fundamentais. Todas as decisões e implementações devem estar alinhadas com este manifesto.

## Fases Detalhadas de Implementação (Fase Inicial)

### Fase 1: Configuração do Ambiente do Servidor (TFS 1.5 Downgrade 8.60)

1.  **Preparação do Ambiente:**
    *   Instalar as dependências necessárias para compilar o TFS em um ambiente Linux (Ubuntu). Isso geralmente inclui `build-essential`, `cmake`, `libboost-all-dev`, `liblua5.3-dev`, `libmysqlclient-dev` (ou equivalente para TiDB), `libsqlite3-dev`, `libxml2-dev`, `libgmp-dev`, `libjsoncpp-dev`, `libz-dev`, `libcrypto++-dev`.
    *   Instalar um servidor MySQL ou TiDB e criar um banco de dados para o Nexoria Online.

2.  **Obtenção e Compilação do Servidor:**
    *   Clonar o repositório `nekiro/TFS-1.5-Downgrades` na branch `8.60` para `/home/ubuntu/nexoria_online/server`.
    *   Configurar o CMake para gerar os arquivos de build.
    *   Compilar o servidor.

3.  **Configuração Inicial do Servidor:**
    *   Copiar `config.lua.dist` para `config.lua` e ajustar as configurações do banco de dados e outras configurações essenciais.
    *   Importar o `schema.sql` para o banco de dados criado.
    *   Executar o servidor pela primeira vez para verificar a funcionalidade básica e a conexão com o banco de dados.

### Fase 2: Configuração do Ambiente do Cliente (OTClientV8 para Android)

1.  **Preparação do Ambiente Android:**
    *   Instalar o Android Studio, Android SDK e Android NDK (se ainda não estiverem presentes no ambiente).
    *   Configurar as variáveis de ambiente necessárias para o NDK e SDK.

2.  **Obtenção e Compilação do Cliente:**
    *   Clonar o repositório `OTCv8/otclientv8` para `/home/ubuntu/nexoria_online/client`.
    *   Seguir as instruções de compilação para Android do OTClientV8 (pesquisar na wiki ou issues do projeto se necessário) para gerar um arquivo `.apk`.
    *   Instalar o `.apk` em um emulador Android ou dispositivo físico para testes.

3.  **Conexão e Teste Básico:**
    *   Configurar o cliente Android para se conectar ao servidor TFS local (ou IP externo, se aplicável).
    *   Verificar se o jogo carrega, se o login funciona e se o personagem pode ser visualizado no mundo.

### Fase 3: Adaptações Iniciais para Mobile (Joystick Touch e UI)

1.  **Análise e Refinamento do Joystick Touch:**
    *   Analisar o arquivo `client/modules/client_mobile/mobile.lua` para entender a lógica do joystick virtual.
    *   Identificar as variáveis e funções responsáveis pela sensibilidade e área de detecção do toque.
    *   Propor e implementar ajustes para otimizar a resposta do joystick para diferentes tamanhos de tela e preferências do usuário.

2.  **Adaptação da Interface do Usuário (UI):**
    *   Analisar os arquivos `.otui` e scripts Lua relevantes para a interface do usuário (ex: barras de vida/mana, inventário, janelas de chat, botões de magia).
    *   Propor um layout inicial para a UI mobile, considerando a ergonomia e a facilidade de uso em telas touch.
    *   Implementar as primeiras modificações na UI para criar botões maiores e mais acessíveis para ações comuns (ex: atacar, usar poção, abrir inventário).

## Entregáveis da Fase Inicial

Ao final desta fase, os seguintes itens devem ser entregues:

1.  **Servidor TFS 8.60 Compilado e Funcional:** Um servidor rodando localmente, acessível e com o banco de dados configurado.
2.  **Cliente OTClientV8 Android Compilado:** Um arquivo `.apk` do cliente OTClientV8 que pode ser instalado e executado em um dispositivo Android.
3.  **Conexão Cliente-Servidor Estabelecida:** Demonstração de que o cliente Android consegue se conectar ao servidor e exibir o mundo do jogo.
4.  **Joystick Touch Básico Funcional:** O joystick virtual deve permitir o movimento do personagem de forma responsiva.
5.  **Documentação de Setup:** Um guia claro e conciso sobre como replicar o ambiente de desenvolvimento (servidor e cliente).
6.  **Relatório de Adaptações Iniciais:** Detalhes das modificações feitas no joystick e na UI, com capturas de tela ou descrições visuais, se possível.

## Restrições e Considerações

*   **Prioridade:** A estabilidade e a funcionalidade básica são prioritárias nesta fase. Otimizações avançadas e novas funcionalidades serão abordadas em etapas futuras.
*   **Modularidade:** Manter o código o mais modular possível para facilitar futuras customizações e atualizações.
*   **Testes:** Realizar testes contínuos para garantir que as modificações não introduzam bugs ou problemas de performance.
*   **Comunicação:** Manter o usuário informado sobre o progresso, desafios e decisões tomadas.

Com este prompt mestre, o agente terá uma compreensão clara do que precisa ser feito para iniciar a construção do Nexoria Online.
