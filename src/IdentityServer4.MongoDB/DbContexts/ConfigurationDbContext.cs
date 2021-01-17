// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.MongoDB.Configuration;
using IdentityServer4.MongoDB.Entities;
using IdentityServer4.MongoDB.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace IdentityServer4.MongoDB.DbContexts
{
    public class ConfigurationDbContext : MongoDBContextBase, IConfigurationDbContext
    {
        private readonly IMongoCollection<ApiResource> _apiResources;
        private readonly IMongoCollection<ApiScope> _apiScopes;
        private readonly IMongoCollection<Client> _clients;
        private readonly IMongoCollection<IdentityResource> _identityResources;

        public ConfigurationDbContext(IOptions<MongoDBConfiguration> settings)
            : base(settings)
        {
            _clients = Database.GetCollection<Client>(Constants.TableNames.Client);
            _identityResources = Database.GetCollection<IdentityResource>(Constants.TableNames.IdentityResource);
            _apiResources = Database.GetCollection<ApiResource>(Constants.TableNames.ApiResource);
            _apiScopes = Database.GetCollection<ApiScope>(Constants.TableNames.ApiScope);

            CreateClientsIndexes();
            CreateIdentityResourcesIndexes();
            CreateApiResourcesIndexes();
            CreateApiScopesIndexes();
        }

        public IQueryable<Client> Clients => _clients.AsQueryable();

        public IQueryable<IdentityResource> IdentityResources => _identityResources.AsQueryable();

        public IQueryable<ApiResource> ApiResources => _apiResources.AsQueryable();

        public IQueryable<ApiScope> ApiScopes => _apiScopes.AsQueryable();

        public async Task AddClient(Client entity)
        {
            await _clients.InsertOneAsync(entity);
        }

        public async Task AddIdentityResource(IdentityResource entity)
        {
            await _identityResources.InsertOneAsync(entity);
        }

        public async Task AddApiResource(ApiResource entity)
        {
            await _apiResources.InsertOneAsync(entity);
        }

        public async Task AddApiScope(ApiScope entity)
        {
            await _apiScopes.InsertOneAsync(entity);
        }

        private void CreateClientsIndexes()
        {
            CreateIndexOptions indexOptions = new CreateIndexOptions {Background = true};

            IndexKeysDefinitionBuilder<Client> builder = Builders<Client>.IndexKeys;
            CreateIndexModel<Client> clientIdIndexModel =
                new CreateIndexModel<Client>(builder.Ascending(_ => _.ClientId), indexOptions);
            _clients.Indexes.CreateOne(clientIdIndexModel);
        }

        private void CreateIdentityResourcesIndexes()
        {
            CreateIndexOptions indexOptions = new CreateIndexOptions {Background = true};

            IndexKeysDefinitionBuilder<IdentityResource> builder = Builders<IdentityResource>.IndexKeys;
            CreateIndexModel<IdentityResource> nameIndexModel =
                new CreateIndexModel<IdentityResource>(builder.Ascending(_ => _.Name), indexOptions);
            _identityResources.Indexes.CreateOne(nameIndexModel);
        }

        private void CreateApiResourcesIndexes()
        {
            CreateIndexOptions indexOptions = new CreateIndexOptions {Background = true};

            IndexKeysDefinitionBuilder<ApiResource> builder = Builders<ApiResource>.IndexKeys;
            CreateIndexModel<ApiResource> nameIndexModel =
                new CreateIndexModel<ApiResource>(builder.Ascending(_ => _.Name), indexOptions);
            CreateIndexModel<ApiResource> scopesIndexModel =
                new CreateIndexModel<ApiResource>(builder.Ascending(_ => _.Scopes), indexOptions);
            _apiResources.Indexes.CreateOne(nameIndexModel);
            _apiResources.Indexes.CreateOne(scopesIndexModel);
        }

        private void CreateApiScopesIndexes()
        {
            CreateIndexOptions indexOptions = new CreateIndexOptions {Background = true};

            IndexKeysDefinitionBuilder<ApiScope> builder = Builders<ApiScope>.IndexKeys;
            CreateIndexModel<ApiScope> nameIndexModel =
                new CreateIndexModel<ApiScope>(builder.Ascending(_ => _.Name), indexOptions);
            _apiScopes.Indexes.CreateOne(nameIndexModel);
        }
    }
}