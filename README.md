# Sistema de login com Minimal API (Projeto em desenvolvimento)
Este é um projeto fullstack de login que utiliza React no frontend e Minimal API no backend. O objetivo principal é criar um sistema robusto para cadastro de usuários, proporcionando uma base sólida que pode ser reutilizada em futuras aplicações. Este projeto também serve como um ambiente de treinamento, permitindo a aplicação prática dos conceitos aprendidos em cursos sobre APIs.

## Funcionalidades
* __Cadastro de Usuários:__ Implementado utilizando o Identity Framework para facilitar a gestão de identidades e informações do usuário.

* __Autenticação:__ Utiliza JWT (JSON Web Token) como um meio seguro de autenticação, proporcionando uma camada adicional de segurança.

* __Controle de Acesso:__ Gerenciado através das Roles do Identity, permitindo a definição de políticas de autorização personalizadas com base nas funções do usuário.

## Minimal API ASP.NET
Este projeto utiliza o framework ASP.NET com foco em uma abordagem minimalista, incorporando as seguintes tecnologias e práticas:

#### Tecnologias Principais:
* __Identity Framework + Entity Framework:__ Integração para gestão de identidade e mapeamento objeto-relacional para interação com o banco de dados.

* __Authentication JWT Bearer:__ Implementação de autenticação segura utilizando JSON Web Tokens (JWT).

* __AutoMapper:__ Facilita a mapeação entre objetos de diferentes modelos, simplificando a manipulação de dados.

* __SQL Server:__ Banco de dados utilizado para armazenamento persistente de informações.

* __Nunit:__ Para testes de integração. 

#### Inicialização da API:
* Ao ser inicializada, a API verifica a existência das roles "Admin" e "User". Se não existirem, são adicionadas automaticamente.

* Verifica se existe um usuário Admin e UserPadrao. Caso não exista, é criado com uma senha padrão. 

#### Endpoints:
* __/Registro:__ Responsável pelo cadastro de usuários. Deve receber os parâmetros username, password e repassword no corpo da requisição. Se algum desses parâmetros estiver em branco, retorna um erro 400.

* __/Login:__ Realiza a autenticação do usuário. Deve receber no corpo da requisição o username e password. Em caso de sucesso, retorna um token JWT.

* __/usuarios:__ Retorna uma lista de todos os usuários cadastrados. Apenas o usuário Admin tem acesso a este endpoint.

* __/usuarios (Delete):__ Apenas o usuário Admin pode acessar. O ID do usuário a ser excluído deve ser enviado no corpo da requisição.

* __/atualizar-dados-usuario:__ Requer um token JWT no cabeçalho da requisição. A atualização é baseada no ID contido no token, e os valores a serem atualizados devem ser fornecidos no corpo da requisição.

* __/alterar-senha:__ Apenas um usuário autenticado pode alterar sua senha. É necessário informar a senha antiga e a nova senha. A senha antiga é verificada; se não for válida, retorna um erro 400.

## Testes de integração
Os testes de integração foi implementado utilizando um banco de dados em memoria, utilizando os usuarios que são criados na inicialização da api para a maioria dos testes.



