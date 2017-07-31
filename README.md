## AzureServiceBus.InterOp

Two NServiceBus endpoints (one using transport encoding `Stream` and one endpoint using transport encoding `byte[]`) send messages to the new Azure Service Bus client (Microsoft.Azure.ServiceBus).
New client then replies back.

Endpoint (`Stream`) ---> new client (works)
Endpoint (`Stream`) <--- new client (works)

Endpoint (`byte[]`) ---> new client (works)
Endpoint (`byte[]`) <--- new client (fails)
