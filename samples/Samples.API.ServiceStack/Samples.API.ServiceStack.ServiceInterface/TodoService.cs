﻿using System.Threading.Tasks;
using CQRSalad.Infrastructure;
using ServiceStack;
using Samples.API.ServiceStack.ServiceModel;
using Samples.Domain.TodoList;

namespace Samples.API.ServiceStack.ServiceInterface
{
    public class TodoService : Service
    {
        private readonly IDomainBus _domainBus;
        private readonly IIdGenerator _idGenerator;

        public TodoService(IDomainBus domainBus, IIdGenerator idGenerator)
        {
            _domainBus = domainBus;
            _idGenerator = idGenerator;
        }
        
        public async Task<CreateTodoListResponse> Any(CreateTodoListRequest request)
        {
            string todoListId = _idGenerator.Generate();

            await _domainBus.CommandAsync(new CreateTodoList
            {
                ListId = todoListId,
                Title = request.Title
            }, ""); 

            return new CreateTodoListResponse
            {
                Id = todoListId
            };
        }
    }
}