using FluentAssertions;
using FluentAssertions.Execution;
using Xunit.Abstractions;
using Grpc.Core;
using Grpc.Net.Client;
using Ozon.Tpl.RfbsProviderTemplate.Grpc;
using Ozon.Tpl.RfbsProviderTemplate.Admin.Grpc;


namespace TestProject1;

public class My_first_test
{
    private readonly ITestOutputHelper output;

    public My_first_test(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public async Task Test1()
    {
        var (client1, client2) = await OpenServiceChannel();
        var res1 = client1.GetBaseRoutesWithConstantDurationsByStartPoint(
            new GetBaseRoutesWithConstantDurationsByStartPointRequest()
            {
                // ProviderId = 51,
                // ProviderTemplateId = 124,
                // RegionFromUid = "061b16ee-90a7-51bf-8af6-491737c8fd62"
                ProviderId = 16,
                ProviderTemplateId = 110,
                RegionFromUid = "0033ed84-6be1-438d-8890-be3087e11b58"

                // ProviderId = 18,
                // ProviderTemplateId = 1,
                // RegionFromUid = "b38157c8-433d-41ba-a89b-a67b7cdd2a87"
            });


        var res2 = client2.GetBaseRoutesWithConstantDurationsByStartPoint(
            new GetBaseRoutesWithConstantDurationsByStartPointRequest()
            {
                // ProviderId = 51,
                // ProviderTemplateId = 124,
                // RegionFromUid = "061b16ee-90a7-51bf-8af6-491737c8fd62"
                ProviderId = 16,
                ProviderTemplateId = 110,
                RegionFromUid = "0033ed84-6be1-438d-8890-be3087e11b58"
                //
                // ProviderId = 18,
                // ProviderTemplateId = 1,
                // RegionFromUid = "b38157c8-433d-41ba-a89b-a67b7cdd2a87"
                // ProviderId = 18,
                // ProviderTemplateId = 22,
                // RegionFromUid = "140e31da-27bf-4519-9ea0-6185d681d44e"
            });

        using (new AssertionScope())
        {
            res1.BaseRoutes.Should().HaveCount(res2.BaseRoutes.Count);
            res1.BaseRoutes.OrderBy(x => x.RegionToUid).Should().Equal(res2.BaseRoutes.OrderBy(x => x.RegionToUid));
            res1.BaseRoutes.OrderBy(x => x.RegionToUid).Should()
                .BeEquivalentTo(res2.BaseRoutes.OrderBy(x => x.RegionToUid));
        }

        output.WriteLine($"TPLRFBS-3297: {res1}\nМастер :{res2}");
    }

    [Fact] 
    public async Task GetBaseRoutesWithConstantDurationsByStartPoint_DurationMaxFromSettings()
    {
        var (client1, client2) = await OpenServiceChannel();
        var res1 = client1.GetBaseRoutesWithConstantDurationsByStartPoint(
            new GetBaseRoutesWithConstantDurationsByStartPointRequest()
            {
                ProviderId = 16,
                ProviderTemplateId = 110,
                RegionFromUid = "0033ed84-6be1-438d-8890-be3087e11b58"
            });
        var res2 = client2.GetBaseRoutesWithConstantDurationsByStartPoint(
            new GetBaseRoutesWithConstantDurationsByStartPointRequest()
            {
                ProviderId = 16,
                ProviderTemplateId = 110,
                RegionFromUid = "0033ed84-6be1-438d-8890-be3087e11b58"
            });

        using (new AssertionScope())
        {
            res2.BaseRoutes.Should().Contain(x => x.DurationMax == 5);
            res1.BaseRoutes.Should().Contain(x => x.DurationMax == 6);
        }

        output.WriteLine($"TPLRFBS-3297: {res1}\nМастер :{res2}");
    }

    [Theory] 
    [InlineData(16,4,4, "0033ed84-6be1-438d-8890-be3087e11b58",
        "b73b7c50-112e-4b04-82df-1e0cea13de4b",true)]
    public async Task GetConstantBaseRouteDurations_DurationMaxFromSettings
        (int providerId, int durMax, int durMin, string fromUid, string toUid, bool isInternal)
    {
        var (client1, client2) = await OpenServiceChannel();
        var res1 = client1.GetConstantBaseRouteDurations(new GetConstantBaseRouteDurationsRequest()
        {
            DurationBaseRoutes =
            {
                new GetConstantBaseRouteDurationsRequest.Types.ConstantBaseRouteDurationRequest()
                {
                    ProviderId = providerId,
                    DurationMax = durMax,
                    DurationMin = durMin,
                    FromUid = fromUid,
                    ToUid = toUid,
                    IsInternalRoute = isInternal
                }
            }
        });
        var res2 = client2.GetConstantBaseRouteDurations(new GetConstantBaseRouteDurationsRequest()
        {
            DurationBaseRoutes =
            {
                new GetConstantBaseRouteDurationsRequest.Types.ConstantBaseRouteDurationRequest()
                {
                    ProviderId = providerId,
                    DurationMax = durMax,
                    DurationMin = durMin,
                    FromUid = fromUid,
                    ToUid = toUid,
                    IsInternalRoute = isInternal
                }
            }
        });
        using (new AssertionScope())
        {
            res2.DurationBaseRoutes.Should().Contain(x => x.ConstantDurationMax == 5);
            res1.DurationBaseRoutes.Should().Contain(x => x.ConstantDurationMax == 6);
        }

        output.WriteLine($"TPLRFBS-3297: {res1}\nМастер :{res2}");
    }

    [Fact] 
    public async Task Test_for_me_5409()
    {
        var (client1, client2) = await OpenServiceChannel();
        var res1 = client1.GetProviderTemplateById(new GetProviderTemplateByIdRequest()
        {
            Id = 100
        });
        var res2 = client2.GetProviderTemplateById(new GetProviderTemplateByIdRequest()
        {
            Id = 100
        });
        res1.Should().BeEquivalentTo(res2);
        output.WriteLine($"TPLRFBS-5409: {res1}\nМастер :{res2}");
    }

    [Fact] 
    public async Task ChangeBaseRouteActivity_ShouldBeSameBothResponses()
    {
        var (client1, client2) = await OpenServiceChannelAdmin();
        var res1 = client1.ChangeBaseRouteActivity(new ChangeBaseRouteActivityRequest
        {
            ProviderId = 132,
            RegionFromUid = "c71b4655-4ec9-5563-afe3-884294efa592",
            RegionToUid = "026bc56f-3731-48e9-8245-655331f596c0",
            ServiceCode = "ACIPOUD",
            IsActive = false
        });
        await Task.Delay(5000);
        var res2 = client2.ChangeBaseRouteActivity(new ChangeBaseRouteActivityRequest
        {
            ProviderId = 132,
            RegionFromUid = "c71b4655-4ec9-5563-afe3-884294efa592",
            RegionToUid = "026bc56f-3731-48e9-8245-655331f596c0",
            ServiceCode = "ACIPOUD",
            IsActive = true
        });
        res1.Should().BeEquivalentTo(res2);
        output.WriteLine($"TPLRFBS-5409: {res1}\nМастер :{res2}");
    }

    [Fact] 
    public async Task ChangeBaseRouteActivityByProviderTemplateId_ShouldBeSameBothResponses()
    {
        var (client1, client2) = await OpenServiceChannelAdmin();

        var res1 = client1.ChangeBaseRouteActivityByProviderTemplateId(
            new ChangeBaseRouteActivityByProviderTemplateIdRequest()
            {
                ProviderId = 132,
                RegionFromUid = "c71b4655-4ec9-5563-afe3-884294efa592",
                RegionToUid = "026bc56f-3731-48e9-8245-655331f596c0",
                ProviderTemplateId = 100,
                IsActive = false
            });
        await Task.Delay(5000);
        var res2 = client2.ChangeBaseRouteActivityByProviderTemplateId(
            new ChangeBaseRouteActivityByProviderTemplateIdRequest()
            {
                ProviderId = 132,
                RegionFromUid = "c71b4655-4ec9-5563-afe3-884294efa592",
                RegionToUid = "026bc56f-3731-48e9-8245-655331f596c0",
                ProviderTemplateId = 100,
                IsActive = true
            });

        res1.Should().BeEquivalentTo(res2);
        output.WriteLine($"TPLRFBS-5409: {res1}\nМастер :{res2}");
    }


    [Fact] 
    public async Task UpdateRouteDurationManual_ShouldBeSameBothResponses()
    {
        var (client1, client2) = await OpenServiceChannelAdmin();

        var res1 = client1.UpdateRouteDurationManual(new UpdateRouteDurationManualRequest
        {
            RegionFromUid = "c71b4655-4ec9-5563-afe3-884294efa592",
            RegionToUid = "026bc56f-3731-48e9-8245-655331f596c0",
            ProviderTemplateId = 100,
            DurationMinManual = 6
        });
        await Task.Delay(5000);
        var res2 = client2.UpdateRouteDurationManual(new UpdateRouteDurationManualRequest
        {
            RegionFromUid = "c71b4655-4ec9-5563-afe3-884294efa592",
            RegionToUid = "026bc56f-3731-48e9-8245-655331f596c0",
            ProviderTemplateId = 100,
            DurationMaxManual = 10,
            DurationMinManual = 20
        });
        res1.Should().BeEquivalentTo(res2);
        output.WriteLine($"TPLRFBS-5409: {res1}\nМастер :{res2}");
    }


    [Fact] 
    public async Task GetBaseRoute_ShouldBeSameBothResponses()
    {
        var (client1, client2) = await OpenServiceChannelAdmin();
        var res1 = client1.GetBaseRoute(new GetBaseRouteRequest
        {
            ProviderId = 132,
            RegionFromUid = "c71b4655-4ec9-5563-afe3-884294efa592",
            RegionToUid = "026bc56f-3731-48e9-8245-655331f596c0",
            ServiceCode = "ACIPOUD",
            CountryFromUid = "c71b4655-4ec9-5563-afe3-884294efa592",
            CountryToUid = "42fb1c32-0cfe-5c96-9fb5-7f8e8449f28c"
        });

        var res2 = client2.GetBaseRoute(new GetBaseRouteRequest
        {
            ProviderId = 132,
            RegionFromUid = "c71b4655-4ec9-5563-afe3-884294efa592",
            RegionToUid = "026bc56f-3731-48e9-8245-655331f596c0",
            ServiceCode = "ACIPOUD",
            CountryFromUid = "c71b4655-4ec9-5563-afe3-884294efa592",
            CountryToUid = "42fb1c32-0cfe-5c96-9fb5-7f8e8449f28c"
        });

        res1.Should().BeEquivalentTo(res2);
        output.WriteLine($"TPLRFBS-5409: {res1}\nМастер :{res2}");
    }

    [Fact] 
    public async Task EditRoute_ShouldBeSameBothResponses()
    {
        var (client1, client2) = await OpenServiceChannel();
        var res1 = client1.EditRoutes(new EditRoutesRequest()
        {
            EditRouteModels =
            {
                new EditRoute()
                {
                    ProviderId = 132,
                    ServiceCode = "ACIPOUD",
                    RegionFromUid = "c71b4655-4ec9-5563-afe3-884294efa592",
                    RegionToUid = "026bc56f-3731-48e9-8245-655331f596c0",
                    DurationMin = 11,
                    DurationMax = 12,
                    IsActive = true
                }
            }
        });
        await Task.Delay(5000);
        var res2 = client2.EditRoutes(new EditRoutesRequest()
        {
            EditRouteModels =
            {
                new EditRoute()
                {
                    ProviderId = 132,
                    ServiceCode = "ACIPOUD",
                    RegionFromUid = "c71b4655-4ec9-5563-afe3-884294efa592",
                    RegionToUid = "026bc56f-3731-48e9-8245-655331f596c0",
                    DurationMin = 10,
                    DurationMax = 15,
                    IsActive = false
                }
            }
        });

        res1.Should().BeEquivalentTo(res2);
        output.WriteLine($"TPLRFBS-5409: {res1}\nМастер :{res2}");
    }

    [Fact] 
    public async Task AddOrUpdateRoutesByTemplateId_ShouldBeSameBothResponses()
    {
        var (client1, client2) = await OpenServiceChannel();
        var res1 = client1.AddOrUpdateRoutesByTemplateId(new AddOrUpdateRoutesByTemplateIdRequest()
        {
            TemplateId = 100,
            EditRouteModels =
            {
                new EditRoute()
                {
                    ProviderId = 132,
                    ServiceCode = "ACIPOUD",
                    DurationMin = 11,
                    DurationMax = 14,
                    RegionFromUid = "c71b4655-4ec9-5563-afe3-884294efa592",
                    RegionToUid = "026bc56f-3731-48e9-8245-655331f596c0",
                    IsActive = true
                }
            }
        });
        await Task.Delay(5000);
        var res2 = client2.AddOrUpdateRoutesByTemplateId(new AddOrUpdateRoutesByTemplateIdRequest()
        {
            TemplateId = 100,
            EditRouteModels =
            {
                new EditRoute()
                {
                    ProviderId = 132,
                    ServiceCode = "ACIPOUD",
                    DurationMin = 10,
                    DurationMax = 15,
                    RegionFromUid = "c71b4655-4ec9-5563-afe3-884294efa592",
                    RegionToUid = "026bc56f-3731-48e9-8245-655331f596c0",
                    Source = 2,
                }
            }
        });
        res1.Should().BeEquivalentTo(res2);
        output.WriteLine($"TPLRFBS-5409: {res1}\nМастер :{res2}");
    }

    [Fact] 
    public async Task DeleteRoutes_ShouldBeSameBothResponses()
    {
        var (client1, client2) = await OpenServiceChannel();
        var res1 = client1.DeleteRoutes(new DeleteRoutesRequest()
        {
            ProviderTemplateId = 100,
            RegionFromUid = "c71b4655-4ec9-5563-afe3-884294efa592",
            RegionToUid = "026bc56f-3731-48e9-8245-655331f596c0"
        });

        await Task.Delay(3000);
        client1.EditRoutes(new EditRoutesRequest()
        {
            EditRouteModels =
            {
                new EditRoute()
                {
                    ProviderId = 132,
                    ServiceCode = "ACIPOUD",
                    RegionFromUid = "c71b4655-4ec9-5563-afe3-884294efa592",
                    RegionToUid = "026bc56f-3731-48e9-8245-655331f596c0",
                    DurationMin = 10,
                    DurationMax = 15,
                    IsActive = true
                }
            }
        });
        await Task.Delay(5000);
        var res2 = client2.DeleteRoutes(new DeleteRoutesRequest()
        {
            ProviderTemplateId = 100,
            RegionFromUid = "c71b4655-4ec9-5563-afe3-884294efa592",
            RegionToUid = "026bc56f-3731-48e9-8245-655331f596c0"
        });
        res1.Should().BeEquivalentTo(res2);
        output.WriteLine($"TPLRFBS-5409: {res2}\nМастер :{res2}");
    }

    private async Task<(RfbsProviderTemplateApi.RfbsProviderTemplateApiClient,
        RfbsProviderTemplateApi.RfbsProviderTemplateApiClient)> OpenServiceChannel()
    {
        var chan1 = GrpcChannel.ForAddress(
            "http://tpl-api-rfbs-providertemplate-release-tplrfbs-3297.tpl.svc.stg.k8s.o3.ru:82", new GrpcChannelOptions
            {
                MaxReceiveMessageSize = null,
                MaxSendMessageSize = null,
                Credentials = ChannelCredentials.Insecure
            });

        var chan2 = GrpcChannel.ForAddress(
            "http://tpl-api-rfbs-providertemplate-release-tplrfbs-5409.tpl.svc.stg.k8s.o3.ru:82", new GrpcChannelOptions
            {
                MaxReceiveMessageSize = null,
                MaxSendMessageSize = null,
                Credentials = ChannelCredentials.Insecure
            });

        var client1 = new RfbsProviderTemplateApi.RfbsProviderTemplateApiClient(chan1);
        var client2 = new RfbsProviderTemplateApi.RfbsProviderTemplateApiClient(chan2);

        return (client1, client2);
    }


    private async Task<(RfbsProviderTemplateAdminApi.RfbsProviderTemplateAdminApiClient,
        RfbsProviderTemplateAdminApi.RfbsProviderTemplateAdminApiClient)> OpenServiceChannelAdmin()
    {
        var chan1 = GrpcChannel.ForAddress(
            "http://tpl-api-rfbs-providertemplate-release-tplrfbs-5409.tpl.svc.stg.k8s.o3.ru:82", new GrpcChannelOptions
            {
                MaxReceiveMessageSize = null,
                MaxSendMessageSize = null,
                Credentials = ChannelCredentials.Insecure
            });

        var chan2 = GrpcChannel.ForAddress("http://tpl-api-rfbs-providertemplate-latest.tpl.svc.stg.k8s.o3.ru:82",
            new GrpcChannelOptions
            {
                MaxReceiveMessageSize = null,
                MaxSendMessageSize = null,
                Credentials = ChannelCredentials.Insecure
            });

        var client1 = new RfbsProviderTemplateAdminApi.RfbsProviderTemplateAdminApiClient(chan1);
        var client2 = new RfbsProviderTemplateAdminApi.RfbsProviderTemplateAdminApiClient(chan2);

        return (client1, client2);
    }
}
