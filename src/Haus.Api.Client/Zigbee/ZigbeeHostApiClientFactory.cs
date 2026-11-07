using System.Net.Http;
using Haus.Api.Client.Options;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client.Zigbee;

public interface IZigbeeHostApiClientFactory
{
    IZigbeeHostApiClient Create();
}

public class ZigbeeHostApiClientFactory(
    IHttpClientFactory httpClientFactory,
    IOptions<ZigbeeHostApiClientSettings> options
) : IZigbeeHostApiClientFactory
{
    public IZigbeeHostApiClient Create()
    {
        return new ZigbeeHostApiClient(httpClientFactory.CreateClient(ZigbeeHostApiClientNames.Default), options);
    }
}
