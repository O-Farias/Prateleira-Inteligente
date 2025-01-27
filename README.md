# 🏪 Prateleira Inteligente

Sistema inteligente para gerenciamento de estoque e controle de validade de produtos, desenvolvido com .NET 8 e arquitetura limpa.

## 📋 Índice

- [Sobre o Projeto](#-sobre-o-projeto)
- [Funcionalidades](#-funcionalidades)
- [Tecnologias](#-tecnologias)
- [Arquitetura](#-arquitetura)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Começando](#-começando)
- [API Endpoints](#-api-endpoints)
- [Testes](#-testes)
- [Licença](#-licença)

## 🎯 Sobre o Projeto

O Prateleira Inteligente é um sistema robusto desenvolvido para automatizar e otimizar o gerenciamento de estoque, especialmente focado no controle de validade de produtos. Ideal para supermercados, farmácias e estabelecimentos que precisam de um controle rigoroso de seus produtos.

## ✨ Funcionalidades

- **Gestão de Produtos**
    - Cadastro completo de produtos
    - Controle de estoque
    - Monitoramento de validade
    - Categorização de produtos

- **Gestão de Prateleiras**
    - Organização por localização
    - Controle de capacidade
    - Alocação inteligente de produtos

- **Sistema de Alertas**
    - Notificações de produtos próximos ao vencimento
    - Alertas de estoque baixo
    - Monitoramento em tempo real

- **Movimentação de Estoque**
    - Registro de entradas e saídas
    - Histórico de movimentações
    - Rastreabilidade completa

## 🛠 Tecnologias

- **.NET 8**
- **Entity Framework Core**
- **SQL Server**
- **xUnit**
- **Swagger**
- **AutoMapper**
- **FluentValidation**

## 📐 Arquitetura

O projeto segue os princípios da Arquitetura Limpa (Clean Architecture):

- **Domain**: Entidades e regras de negócio
- **Infrastructure**: Implementação de persistência e serviços externos
- **Application**: Casos de uso e lógica de aplicação
- **API**: Controllers e endpoints REST

## 🗂 Estrutura do Projeto

```
src/
├── PrateleiraInteligente.API/
├── PrateleiraInteligente.Domain/
├── PrateleiraInteligente.Infrastructure/
tests/
└── PrateleiraInteligente.Tests/
```

## 🚀 Começando

1. **Pré-requisitos**
     ```bash
     .NET 8.0 SDK
     SQL Server
     ```

2. **Clone o repositório**
     ```bash
     git clone https://github.com/O-Farias/Prateleira-Inteligente.git
     ```

3. **Configure o banco de dados**
     ```bash
     cd src/PrateleiraInteligente.API
     dotnet ef database update
     ```

4. **Execute o projeto**
     ```bash
     dotnet run
     ```

## 📡 API Endpoints

### Produtos
- `GET /api/produtos` - Lista todos os produtos
- `GET /api/produtos/{id}` - Obtém um produto específico
- `POST /api/produtos` - Cria um novo produto
- `PUT /api/produtos/{id}` - Atualiza um produto
- `DELETE /api/produtos/{id}` - Remove um produto

### Categorias
- `GET /api/categorias` - Lista todas as categorias
- `GET /api/categorias/{id}` - Obtém uma categoria específica
- `POST /api/categorias` - Cria uma nova categoria
- `PUT /api/categorias/{id}` - Atualiza uma categoria
- `DELETE /api/categorias/{id}` - Remove uma categoria

### Movimentações
- `GET /api/movimentacoes/produto/{produtoId}` - Lista movimentações de um produto
- `POST /api/movimentacoes/entrada` - Registra entrada de produtos
- `POST /api/movimentacoes/saida` - Registra saída de produtos

### Prateleiras
- `GET /api/prateleiras` - Lista todas as prateleiras
- `POST /api/prateleiras` - Cria uma nova prateleira
- `GET /api/prateleiras/{id}/espaco-disponivel` - Verifica espaço disponível

### Alertas
- `GET /api/alertas` - Lista alertas não resolvidos
- `POST /api/alertas/verificar-vencimentos` - Verifica produtos próximos ao vencimento
- `PUT /api/alertas/{id}/resolver` - Resolve um alerta

## 🧪 Testes

O projeto inclui testes unitários e de integração:

```bash
cd tests/PrateleiraInteligente.Tests
dotnet test
```

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.