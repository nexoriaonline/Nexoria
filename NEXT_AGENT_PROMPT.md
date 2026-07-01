# 🚀 PROMPT PARA O PRÓXIMO AGENTE - NEXORIA ONLINE

## ⚠️ IMPORTANTE: Leia Isto Primeiro

Este é um projeto de **MMORPG Web baseado em Tibia 8.60**. O agente anterior (eu) preparou a base técnica, mas **você precisará criar seu próprio ambiente de testes local** pois os servidores temporários expiram quando a sessão termina.

---

## 📥 Acesso ao Repositório

```
Link: https://github.com/nexoriaonline/Nexoria.git
Email: nexoriaonline@gmail.com
Nome: Nexoria Online
```

**IMPORTANTE:** Peça o token de acesso ao proprietário do projeto (não está incluído aqui por segurança).

### Configuração Git Inicial
```bash
git config --global user.email "nexoriaonline@gmail.com"
git config --global user.name "Nexoria Online"
git clone https://github.com/nexoriaonline/Nexoria.git
cd Nexoria
git pull origin main
```

---

## 🎯 Sua Missão

**Objetivo Principal:** Fazer o **Nexoria Online Web funcionar de verdade** com o servidor TFS e o cliente Web se comunicando.

Isso significa:
- ✅ Servidor TFS rodando localmente
- ✅ Cliente Web acessível localmente
- ✅ Login funcional (credenciais `1/1`)
- ✅ Personagem aparecendo no mapa
- ✅ Movimento básico funcionando
- ✅ Monstros e combate básico

---

## 📋 Checklist de Configuração Inicial

### Passo 1: Clonar e Analisar
- [ ] Clonar o repositório
- [ ] Ler `PROGRESS.md` para entender o que foi feito
- [ ] Examinar a estrutura de pastas
- [ ] Verificar os arquivos do servidor em `/server/`
- [ ] Verificar os arquivos do cliente em `/client/web/`

### Passo 2: Configurar o Servidor TFS Localmente
- [ ] Navegar para `/server/`
- [ ] Verificar se o executável `tfs` existe (se não, compilar com CMake)
- [ ] Verificar se MariaDB está instalado (se não, instalar)
- [ ] Iniciar o servidor: `./tfs`
- [ ] Verificar logs em `server.log` para erros
- [ ] Confirmar que o servidor está online (deve dizer "Forgotten Server Online!")

### Passo 3: Configurar o Cliente Web Localmente
- [ ] Navegar para `/client/web/`
- [ ] Iniciar um servidor web local: `python3 -m http.server 8080`
- [ ] Abrir no navegador: `http://localhost:8080`
- [ ] Verificar se a interface de login aparece
- [ ] Testar o joystick e os botões

### Passo 4: Conectar Cliente ao Servidor (CRÍTICO)
- [ ] Implementar WebSocket no `index.html`
- [ ] Conectar ao servidor TFS na porta 7171 (login)
- [ ] Implementar protocolo de autenticação
- [ ] Testar login com credenciais `1/1`
- [ ] Se funcionar, o personagem deve aparecer no mapa

### Passo 5: Implementar Renderização de Gráficos
- [ ] Descompactar `Nexoria.spr` e `Nexoria.dat`
- [ ] Converter sprites para WebGL/Canvas
- [ ] Renderizar o mapa
- [ ] Renderizar o personagem do jogador
- [ ] Renderizar monstros

### Passo 6: Implementar Movimento e Combate
- [ ] Capturar eventos do joystick
- [ ] Enviar comandos de movimento para o servidor
- [ ] Implementar ataque básico
- [ ] Testar combate com monstros

### Passo 7: Fazer Commit e Push
- [ ] Testar tudo localmente
- [ ] Fazer commit com mensagem descritiva
- [ ] Fazer push para o GitHub

---

## 📂 Estrutura do Repositório

```
Nexoria/
├── server/
│   ├── src/                    # Código-fonte C++ do TFS
│   ├── build/                  # Arquivos compilados
│   ├── tfs                     # Executável (já compilado)
│   ├── config.lua              # Configuração do servidor
│   ├── data/                   # Dados do servidor (mapas, spawns, etc)
│   └── server.log              # Log do servidor
├── client/
│   ├── web/                    # Cliente Web (HTML/CSS/JS)
│   │   └── index.html          # Interface principal
│   ├── modules/                # Módulos Lua (OTClient)
│   ├── data/
│   │   └── things/860/
│   │       ├── Nexoria.dat     # Dados dos objetos
│   │       └── Nexoria.spr     # Sprites dos gráficos
│   └── Nexoria_Definitivo_V4.apk  # APK Android (opcional)
├── docs/                       # Documentação
├── PROGRESS.md                 # Progresso do projeto
└── NEXT_AGENT_PROMPT.md        # Este arquivo
```

---

## 🔧 Instalação de Dependências

### Para Compilar o Servidor (se necessário)
```bash
sudo apt-get update
sudo apt-get install -y build-essential cmake git libmariadb-dev libluajit-2.1-dev
cd server
mkdir build && cd build
cmake ..
make -j$(nproc)
```

### Para Rodar o Servidor
```bash
sudo apt-get install -y mariadb-server
sudo service mariadb start
cd server
./tfs
```

### Para o Cliente Web
```bash
# Já vem com Python
python3 -m http.server 8080 --directory /path/to/client/web
```

---

## 🌐 Endereços Locais (Você Criará)

Quando você estiver testando localmente:

```
Servidor TFS:
  - IP: 127.0.0.1
  - Login Port: 7171
  - Game Port: 7172

Cliente Web:
  - URL: http://localhost:8080
  - WebSocket Login: ws://127.0.0.1:8001
  - WebSocket Game: ws://127.0.0.1:8002
```

**Nota:** Você precisará configurar um proxy WebSocket (websockify) para encaminhar as conexões WebSocket para as portas TCP do servidor.

---

## 🔐 Credenciais de Teste

```
Account: 1
Password: 1
```

Essas credenciais estão pré-configuradas no banco de dados do servidor.

---

## 💡 Dicas Críticas

### 1. Servidor Sempre Rodando
O servidor TFS deve estar rodando em um terminal enquanto você testa o cliente:
```bash
cd /home/ubuntu/Nexoria/server
./tfs
```

### 2. Verificar Logs
Se algo não funcionar, sempre verifique os logs:
```bash
tail -f /home/ubuntu/Nexoria/server/server.log
```

### 3. WebSocket é Essencial
Sem WebSocket, o cliente Web não consegue se comunicar com o servidor TFS. Você PRECISA implementar isso.

### 4. Protocolo Tibia 8.60
O servidor TFS usa o protocolo Tibia 8.60. Você precisará entender como esse protocolo funciona para implementar o cliente Web corretamente.

### 5. Sprites Compactados
Os arquivos `.spr` e `.dat` estão compactados. Você precisará descompactá-los antes de renderizar.

---

## 📚 Recursos Úteis

- **Tibia Protocol:** Pesquise "Tibia 8.60 Protocol" para entender como funciona
- **OTClient:** O código-fonte do OTClient pode ajudar a entender como renderizar sprites
- **WebSocket:** Use a biblioteca `ws` do Node.js ou `WebSocket` nativo do navegador
- **Canvas/WebGL:** Para renderização gráfica no navegador

---

## 🚨 Problemas Comuns

### "Servidor Offline"
- Verifique se o servidor TFS está rodando
- Verifique se MariaDB está rodando
- Verifique os logs do servidor

### "Conexão Recusada"
- Verifique se as portas 7171 e 7172 estão abertas
- Verifique se o WebSocket proxy está rodando
- Verifique o firewall

### "Sprite não aparece"
- Verifique se os arquivos `.spr` e `.dat` foram descompactados corretamente
- Verifique se o caminho para os arquivos está correto
- Verifique o console do navegador para erros

---

## 📝 Recomendações para Continuação

1. **Prioridade 1:** Fazer o servidor TFS rodar localmente
2. **Prioridade 2:** Fazer o cliente Web se conectar ao servidor via WebSocket
3. **Prioridade 3:** Implementar login real
4. **Prioridade 4:** Renderizar sprites Tibia
5. **Prioridade 5:** Implementar movimento
6. **Prioridade 6:** Implementar combate

---

## 🎯 Quando Tudo Estiver Funcionando

Depois que você conseguir fazer o jogo funcionar localmente:

1. **Fazer commit e push** para o GitHub
2. **Atualizar o PROGRESS.md** com o que foi feito
3. **Preparar um novo prompt** para o próximo agente (se necessário)
4. **Considerar migração para VPS** com IP fixo para hospedagem permanente

---

## 📞 Dúvidas?

Se você tiver dúvidas sobre a arquitetura ou sobre como proceder, consulte:
- `PROGRESS.md` - Status atual do projeto
- `server/README.md` - Instruções do servidor
- `client/web/index.html` - Código do cliente Web

---

**Boa sorte! O Nexoria Online está esperando por você! 🛡️🌌🚀**

Desenvolvido com ❤️ para o Nexoria Online  
Última atualização: 01/07/2026
