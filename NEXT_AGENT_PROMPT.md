# 📋 PROMPT PARA PRÓXIMO AGENTE - NEXORIA MMORPG

## 🎯 Contexto

Você está continuando o desenvolvimento do **NEXORIA**, um MMORPG de fazenda online multiplayer completo.

**Status atual:** Preparação de terreno concluída ✅

A estrutura básica foi criada e o repositório GitHub foi atualizado. Agora você precisa implementar o **Fase 1 - MVP (Backend + Frontend Básico)**.

## 📍 Localização do Projeto

```
GitHub: https://github.com/nexoriaonline/Nexoria
Local: /home/ubuntu/nexoria-mmorpg
```

## 📊 Estrutura Atual

```
nexoria-mmorpg/
├── backend/          # Vazio - Precisa ser implementado
├── frontend/         # Vazio - Precisa ser implementado
├── README.md         # Documentação básica
└── PROJECT.md        # Especificação completa do projeto
```

## 🚀 Sua Missão - Fase 1: MVP

Você deve implementar o **MVP (Mínimo Produto Viável)** com:

### 1. Backend (Node.js + Express + Socket.io)

**Arquivos a criar:**
- `backend/package.json` - Dependências
- `backend/.env.example` - Variáveis de ambiente
- `backend/src/index.js` - Servidor principal
- `backend/src/routes/` - Rotas da API
- `backend/src/middleware/` - Middlewares (auth, etc)
- `backend/src/services/` - Lógica de negócio
- `backend/src/models/` - Modelos de dados

**Funcionalidades:**
- ✅ Servidor Express rodando
- ✅ WebSocket com Socket.io
- ✅ Autenticação básica (JWT)
- ✅ Rota de registro/login
- ✅ Rota de criar fazenda
- ✅ Rota de plantio
- ✅ Rota de colheita
- ✅ Sincronização multiplayer em tempo real

### 2. Frontend (React + Socket.io)

**Arquivos a criar:**
- `frontend/package.json` - Dependências
- `frontend/src/main.jsx` - Entrada
- `frontend/src/App.jsx` - Componente principal
- `frontend/src/pages/` - Páginas (Login, Farm, etc)
- `frontend/src/components/` - Componentes reutilizáveis
- `frontend/src/hooks/` - Custom hooks
- `frontend/src/services/` - Serviços (API, WebSocket)
- `frontend/vite.config.js` - Configuração Vite

**Funcionalidades:**
- ✅ Tela de login/registro
- ✅ Tela de fazenda
- ✅ Plantio com clique
- ✅ Colheita
- ✅ Sincronização multiplayer
- ✅ Responsivo para mobile

### 3. Banco de Dados (PostgreSQL)

**Tabelas essenciais:**
- `users` - Usuários
- `player_profiles` - Perfis
- `farms` - Fazendas
- `plants` - Plantas

### 4. Testes

- ✅ Testar backend em localhost:3001
- ✅ Testar frontend em localhost:5173
- ✅ Testar multiplayer com 2+ clientes
- ✅ Testar sincronização em tempo real

## 📝 Especificação Completa

Veja `PROJECT.md` para a especificação COMPLETA do projeto incluindo:
- Todas as funcionalidades
- Arquitetura detalhada
- Banco de dados
- Segurança
- Roadmap completo

## 🔧 Tecnologias

**Backend:**
- Node.js
- Express.js
- Socket.io
- PostgreSQL
- JWT
- bcryptjs

**Frontend:**
- React 18+
- Vite
- Socket.io-client
- Tailwind CSS (opcional)

## 📦 Dependências Principais

**Backend:**
```json
{
  "express": "^4.18.2",
  "socket.io": "^4.7.2",
  "pg": "^8.11.3",
  "jsonwebtoken": "^9.1.2",
  "bcryptjs": "^2.4.3",
  "dotenv": "^16.3.1"
}
```

**Frontend:**
```json
{
  "react": "^18.2.0",
  "socket.io-client": "^4.7.2",
  "vite": "^5.0.0"
}
```

## 🎮 Fluxo do Jogo MVP

1. Usuário acessa o site
2. Faz login/registro
3. Cria uma fazenda
4. Vê o grid de plantio
5. Clica para plantar
6. Planta cresce em tempo real
7. Clica para colher
8. Ganha ouro
9. Vê outros jogadores plantando/colhendo em tempo real

## ✅ Checklist

- [ ] Backend inicializado
- [ ] Frontend inicializado
- [ ] Autenticação implementada
- [ ] Banco de dados criado
- [ ] Rota de login/registro
- [ ] Rota de criar fazenda
- [ ] Rota de plantio
- [ ] Rota de colheita
- [ ] WebSocket funcionando
- [ ] Sincronização multiplayer
- [ ] Frontend responsivo
- [ ] Testes funcionando
- [ ] GitHub atualizado

## 🚀 Começar

1. Clone o repositório
2. Navegue para `/home/ubuntu/nexoria-mmorpg`
3. Crie o backend
4. Crie o frontend
5. Teste tudo
6. Faça commit e push
7. Crie um novo NEXT_AGENT_PROMPT.md para a próxima fase

## 📞 Notas Importantes

- Mantenha o código limpo e bem documentado
- Use boas práticas de segurança
- Implemente validação de entrada
- Adicione tratamento de erros
- Faça commits frequentes
- Atualize o README conforme progride
- Crie um novo NEXT_AGENT_PROMPT.md quando terminar

## 🎯 Objetivo Final

Ao final desta fase, você deve ter:
- ✅ Backend funcional com Express + Socket.io
- ✅ Frontend funcional com React
- ✅ Autenticação básica
- ✅ Sistema de fazenda multiplayer básico
- ✅ Sincronização em tempo real
- ✅ Tudo testado e funcionando

---

**Boa sorte! 🌾 Vamos criar o melhor MMORPG de fazenda online!**

Qualquer dúvida, consulte `PROJECT.md` para a especificação completa.
