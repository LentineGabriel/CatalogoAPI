# 📦 CatalogoAPI
API RESTful para gerenciamento de produtos e categorias. Permite cadastrar, editar, visualizar e deletar itens no catálogo, com integração entre produtos e suas categorias.

RESTful API for managing products and categories. It allows you to register, edit, view and delete items in the catalog, with integration between products and their categories.

---

## 🛠 Stack utilizada

- [.NET 10](https://dotnet.microsoft.com/)
- ASP.NET Core Web API
- MySQL
- Swagger
- Postman (testes/tests)

---

## 🚀 Como rodar o projeto / How to run the project
1. Clone o repositório / Clone the repository:
   ```bash
   git clone https://github.com/LentineGabriel/CatalogoAPI.git
   cd CatalogoAPI
   ```

2. Restaure os pacotes / Restore packages:
   ```bash
   dotnet restore
   ```

3. Rode o projeto / Run the project:
   ```bash
   dotnet run
   ```

> A API estará disponível em: `https://localhost:7112/swagger`<br>
> The API will be available at: `https://localhost:7112/swagger`

---

## 📚 Endpoints

### 🔹 Produtos / Products
- **Cadastrar Produto**
  - `POST Produtos/AdicionarProduto`
  - **Body**:
    ```json
    {
      "produtoId": 1,
      "produtoNome": "Lanche de Atum",
      "produtoDescricao": "Lanche de Atum com Maionese",
      "produtoPreco": 8.50,
      "produtoImagemUrl": "lanche-atum.jpg",
      "produtoEstoque": "15",
      "dataCadastro": "yyyy/mm/dd HH:mm:ss",
      "categoriaId": 1
    }
    ```

- **Visualizar Todos os Produtos**
  - `GET /Produtos`

- **Visualizar Produto por ID**
  - `GET /Produtos/{id}`

- **Editar Produto**
  - `PUT /Produtos/AtualizarProduto/{id}`

- **Deletar Produto**
  - `DELETE /Produtos/DeletarProduto/{id}`

---

### 🔹 Categorias

- **Cadastrar Categoria**
  - `POST /Categorias/AdicionarCategoria`
  - **Body**:
    ```json
    {
      "categoriaId": 1,
      "categoriaNome": "Lanches",
      "categoriaImagemUrl": "lanches.jpg"
    }
    ```

- **Visualizar Todas as Categorias**
  - `GET /Categorias`

- **Visualizar Categoria por ID**
  - `GET /Categorias/{id}`

- **Editar Categoria**
  - `PUT /Categorias/AtualizarCategoria/{id}`

- **Deletar Categoria**
  - `DELETE /Categorias/DeletarCategoria/{id}`

- **Visualizar Categoria e seus respectivos Produtos**
  - `GET /Categorias/ComProdutos`

---

### 🔹 Usuários

- **Criar Perfil**
  - `POST /CriarPerfil`

- **Adicionar Usuário ao Perfil**
  - `POST /AdicionarUsuarioAoPerfil`

- **Login**
  - `POST /Login`

- **Registrar Usuário**
  - `POST /RegistrarUsuário`

- **Refresh Token**
  - `POST /RefreshToken`

- **Revogar Token**
  - `POST /RevogarToken/{username}`

---

## ✅ Funcionalidades implementadas

- [x] CRUD de produtos
- [x] CRUD de categorias
- [x] CRUD de usuários
- [x] Relacionamento entre produtos e categorias
- [x] Swagger UI para testes

---

## 🧪 Testes

Você pode testar todos os endpoints usando o Swagger em:
```
https://localhost:7112/swagger
```

Ou importar para o Postman manualmente.

---

## 📌 Observações

- Certifique-se de ter um banco MySQL rodando e configurado.
- Os dados de conexão estão em `appsettings.json`.
- O projeto está em desenvolvimento seguindo o curso presente em meu perfil de estudos: <a href="https://github.com/GabrielLentine/CSharp_APIs">Repositório do Projeto</a>

---

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.
