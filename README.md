# 🚜 Agro.Management.API (Domain Service)

Microsserviço "Core" do negócio. Gerencia o cadastro de fazendas, talhões e culturas.

## 📋 Responsabilidades
- CRUD de Propriedades Rurais.
- Gestão de Talhões (Areas de plantio) dentro das propriedades.
- Consulta de IDs para configuração dos sensores.

## 🛠️ Stack Tecnológica
- .NET 8 Web API
- MongoDB (Driver Nativo)

## 🧩 Modelo de Dados
Utiliza **Embedded Documents** para performance de leitura:
- Uma `Property` contém uma lista de `Fields` (Talhões).
- Isso permite carregar toda a fazenda em uma única query.

## ⚙️ Configuração
```json
{
  "ConnectionStrings": {
    "MongoDb": "mongodb://localhost:27017"
  },
  "DatabaseName": "AgroDB"
}
```