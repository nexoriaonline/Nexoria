# 🚀 PROMPT PARA O PRÓXIMO AGENTE - NEXORIA ONLINE

## Acesso ao Repositório

```
Link: https://github.com/nexoriaonline/Nexoria.git
Token: [SOLICITAR AO PROPRIETÁRIO DO PROJETO]
Email: nexoriaonline@gmail.com
Nome: Nexoria Online
```

### Configuração Git
```bash
git config --global user.email "nexoriaonline@gmail.com"
git config --global user.name "Nexoria Online"
git clone https://github.com/nexoriaonline/Nexoria.git
cd Nexoria
git pull origin main
```

---

## 📊 Status Atual do Projeto

O Nexoria Online é um **MMORPG Web baseado em Tibia 8.60** que está em fase de desenvolvimento ativo. O servidor está compilado e funcionando, o cliente Android foi otimizado, e agora temos um **cliente Web em HTML5/JavaScript** pronto para ser integrado com o servidor.

### ✅ Concluído
- Servidor TFS 8.60 compilado e rodando
- Cliente Android com joystick otimizado
- Cliente Web com interface neon azul
- Rebranding completo (desvinculado do Tibia)
- Hospedagem web local (porta 8080)

### 🚧 Em Progresso
- Integração WebSocket (cliente Web ↔ servidor TFS)
- Renderização de sprites Tibia 8.60
- Sistema de movimento e combate

---

## 🎯 Tarefa Imediata para o Próximo Agente

### Objetivo Principal
**Conectar o cliente Web ao servidor TFS 8.60 e fazer o jogo funcionar de verdade.**

Isso significa:
1. O jogador consegue fazer login com credenciais reais
2. O personagem aparece no mapa
3. O movimento funciona (8 direções)
4. Monstros aparecem e atacam
5. Combate básico funciona

### Passos Recomendados

#### Passo 1: Entender a Arquitetura
- Ler `/home/ubuntu/Nexoria/PROGRESS.md` para entender o que foi feito
- Examinar o servidor TFS em `/home/ubuntu/Nexoria/server/`
- Analisar o cliente Web em `/home/ubuntu/Nexoria/client/web/index.html`

#### Passo 2: Implementar WebSocket no Cliente Web
- Criar um arquivo `client.js` que implemente a lógica de conexão
- Usar a biblioteca `ws` (WebSocket) para conectar ao servidor
- Implementar protocolo de login (enviar account/password)
- Implementar protocolo de game (receber posição, monstros, etc)

#### Passo 3: Descompactar Sprites Tibia
- Extrair arquivos `.spr` e `.dat` do diretório `/home/ubuntu/Nexoria/client/data/things/860/`
- Converter para formato WebGL (PNG/WebP)
- Criar um mapa de sprites em JavaScript

#### Passo 4: Renderizar Gráficos
- Substituir o Canvas simples por WebGL ou Pixi.js
- Renderizar tiles do mapa
- Renderizar personagem do jogador
- Renderizar monstros

#### Passo 5: Implementar Movimento
- Capturar eventos do joystick
- Enviar comando de movimento para o servidor
- Atualizar posição do jogador na tela

---

## 📂 Arquivos Importantes

| Arquivo | Descrição |
|---------|-----------|
| `/home/ubuntu/Nexoria/server/tfs` | Executável do servidor compilado |
| `/home/ubuntu/Nexoria/server/config.lua` | Configuração do servidor |
| `/home/ubuntu/Nexoria/client/web/index.html` | Interface Web principal |
| `/home/ubuntu/Nexoria/client/data/things/860/Nexoria.dat` | Dados dos objetos |
| `/home/ubuntu/Nexoria/client/data/things/860/Nexoria.spr` | Sprites dos gráficos |
| `/home/ubuntu/Nexoria/PROGRESS.md` | Progresso detalhado |

---

## 🔗 Endereços e Credenciais

### Servidor TFS
```
IP: 127.0.0.1
Login Port: 7171
Game Port: 7172
Database: MariaDB (localhost)
```

### Cliente Web (Hospedagem Local)
```
URL: https://8080-i3pwfbaam2ht40ll616nu-1c8e114e.us2.manus.computer
WebSocket Login: wss://8001-i3pwfbaam2ht40ll616nu-1c8e114e.us2.manus.computer
WebSocket Game: wss://8002-i3pwfbaam2ht40ll616nu-1c8e114e.us2.manus.computer
```

### Credenciais de Teste
```
Account: 1
Password: 1
```

---

## 💡 Dicas Importantes

1. **Servidor Sempre Rodando:** O servidor TFS pode ser iniciado com:
   ```bash
   cd /home/ubuntu/Nexoria/server
   ./tfs
   ```

2. **Logs do Servidor:** Verifique `/home/ubuntu/Nexoria/server/server.log` para erros

3. **Protocolo Tibia:** O servidor TFS usa o protocolo Tibia 8.60. Você precisará implementar um cliente que entenda esse protocolo.

4. **WebSocket Proxy:** O websockify está configurado para encaminhar tráfego WebSocket para as portas TCP do servidor.

5. **Rebranding:** Todos os nomes foram alterados para "Nexoria". Mantenha essa identidade ao adicionar novos recursos.

---

## 🎨 Estilo Visual

O projeto usa um **tema Neon Azul (#00d4ff)** com fundo escuro. Mantenha essa identidade visual em todas as novas interfaces.

---

## 📋 Checklist para Continuação

- [ ] Clonar o repositório e fazer `git pull`
- [ ] Iniciar o servidor TFS
- [ ] Analisar o código do cliente Web
- [ ] Implementar WebSocket no cliente
- [ ] Testar conexão com credenciais `1/1`
- [ ] Descompactar sprites Tibia
- [ ] Renderizar primeiro sprite na tela
- [ ] Implementar movimento básico
- [ ] Implementar combate básico
- [ ] Fazer commit e push para GitHub

---

## 🚀 Comandos Úteis

```bash
# Clonar repositório
git clone https://github.com/nexoriaonline/Nexoria.git
cd Nexoria

# Iniciar servidor
cd server
./tfs

# Verificar logs
tail -f server.log

# Fazer commit
git add .
git commit -m "Descrição das mudanças"
git push origin main
```

---

## ❓ Perguntas Frequentes

**P: Como faço para testar o cliente Web?**  
R: Abra o navegador em `https://8080-i3pwfbaam2ht40ll616nu-1c8e114e.us2.manus.computer` e use as credenciais `1/1`.

**P: Como conecto o cliente Web ao servidor?**  
R: Implemente WebSocket no `index.html` para conectar aos endereços `wss://8001...` e `wss://8002...`.

**P: Onde estão os sprites do Tibia?**  
R: Em `/home/ubuntu/Nexoria/client/data/things/860/Nexoria.spr` (arquivo compactado).

**P: Como faço para adicionar novos monstros?**  
R: Edite os arquivos de spawn do servidor em `/home/ubuntu/Nexoria/server/data/spawns/` e reinicie o servidor.

---

**Desenvolvido com ❤️ para o Nexoria Online**  
**Última atualização:** 01/07/2026
