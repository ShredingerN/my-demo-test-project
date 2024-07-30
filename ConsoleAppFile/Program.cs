using System.Text;
using Grpc.Core;
using Grpc.Net.Client;
using Ozon.Tpl.RfbsProviderTemplate.Grpc;

namespace ConsoleAppFile;

public static class Program
{
    public static async Task Main()
    {
        StressTestHandle();
    }

    public static void StressTestHandle()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        Environment.SetEnvironmentVariable("GRPC_DNS_RESOLVER", "native");
        var channel = GrpcChannel.ForAddress("http://tpl-api-rfbs-providertemplate-release-tplrfbs-4918.tpl.svc.stg.k8s.o3.ru:82", new GrpcChannelOptions
        {
            MaxReceiveMessageSize = null,
            MaxSendMessageSize = null,
            Credentials = ChannelCredentials.Insecure
        });
        
        var client = new RfbsProviderTemplateApi.RfbsProviderTemplateApiClient(channel);
        
        var guids = new List<string>
        {
            "13a6e0b2-f138-597d-a3ff-071876394e41",
            "bab5770a-51d5-5998-8eaa-f055905ee2bd",
            "13a6e0b2-f138-597d-a3ff-071876394e41",
            "13a6e0b2-f138-597d-a3ff-071876394e41",
            "bab5770a-51d5-5998-8eaa-f055905ee2bd",
            "13a6e0b2-f138-597d-a3ff-071876394e41",
            "13a6e0b2-f138-597d-a3ff-071876394e41",
            "bab5770a-51d5-5998-8eaa-f055905ee2bd"
        };

        var templates = new List<int>
        {
            194,
            194,
            195,
            201,
            201,
            205,
            301,
            301
        };
        
        var tasks = new Task[templates.Count];
        for (int i = 0; i < templates.Count; ++i)
        {
            var i1 = i;
            tasks[i1] = Task.Run(() =>
            {
                var iterations = 0;
                while (true)
                {
                    try
                    {
                        var res = client.GetBaseRoutesWithConstantDurationsByStartPoint(
                            new GetBaseRoutesWithConstantDurationsByStartPointRequest()
                            {
                                RegionFromUid = guids[i1],
                                ProviderTemplateId = templates[i1]
                            },
                            new Metadata() { { "x-o3-s2s", "Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6Ik5WZE53TkdXZWZJbXdlYUNzRmRxR0oxbDdjanJnSzVwZTZ0bUg5d0JJV1EiLCJ0eXAiOiJKV1QifQ.eyJhY3IiOjEsImF1ZCI6InMyc3x0cGwtYXBpLXJmYnMtcHJvdmlkZXJ0ZW1wbGF0ZSIsImF6cCI6InMyc3xicmVhay10aGUtZ2xhc3MiLCJjbGllbnRJZCI6InMyc3xicmVhay10aGUtZ2xhc3MiLCJleHAiOjE3MDY2MzIxOTQsImlhdCI6MTcwNjYxNzc5NCwiaXNzIjoiaHR0cHM6Ly9zc28ubzMucnUvYXV0aC9yZWFsbXMvc2VydmljZTJzZXJ2aWNlIiwiaXNzdWVkQnlVc2VyIjoibWFiYWxhYmFub3YiLCJqdGkiOiJUb2M2Y050andQRF9VOERfIiwicmVzb3VyY2VfYWNjZXNzIjp7InMyc3x0cGwtYXBpLXJmYnMtcHJvdmlkZXJ0ZW1wbGF0ZSI6eyJyb2xlcyI6WyIqIl19fSwic2NvcGUiOiJzMnN8dHBsLWFwaS1yZmJzLXByb3ZpZGVydGVtcGxhdGUiLCJzdWIiOiJzMnN8YnJlYWstdGhlLWdsYXNzIiwidHlwIjoiQmVhcmVyIn0.s83amwAgfloJvJezOxB5j74m9d3WfBEvH5dA9nHkQYRcQbSfhOksr3VhO83yyY5Ba--7YlzbnYRIvfdNw9lKYleSMbVrMQnJkNampsywr7RgtOzDt3kQ89vr_eeEIdds4d5ov8Sp2nttV7IHjg3WufbrXSjH2isa774g1SccNuv0jcgtn2WyxUQJZG_N6pL8D0KkeGzyY9uLQRZ_tou-j2Tk0jKlsbNExFpS_zYwwJKvwexOobnAxYPcYhiqDJTAA3C2zTMXUDcDWHFeLt4R-i0LIu5QwzjKRKxh3S9LLWqUKWFgm-EYDoieIsyTPgLGWsPIRSbeHWLIzZkEWekqlg" } },
                            deadline: DateTime.MaxValue,
                            cancellationToken: CancellationToken.None);

                        Task.Delay(300).Wait();
                        Console.WriteLine($"{i1}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            });
        }

        Task.WaitAll(tasks);
    }
}