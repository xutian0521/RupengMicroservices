using System;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.Timeout;

namespace PolicyTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //PollyFallbackTest1();
            //PollyFallBackTest2();
            //PollyRetryTest();
            //PollyCircuitBreakerTest();
            //PollyWarpTest();
            //PollyTimeoutTest();
            //await PollyTimeoutTest1Async();
            //await PollyTimeoutTest2Async();
            //PollyCircuitBreakerTest2();
            await PollyCircuitBreakerTest3();
            Console.ReadKey();
        }
        //------------------------------------------降级处理 Fallback--------------------------------------------
        /*
         * Polly 先定义故障类型 如异常故障，然后定义处理策略 如降级 Fallback，然后用Policy 对象执行可能出现故障的代码
         */
        /// <summary>
        /// Polly 异常降级处理1
        /// </summary>
        static void PollyFallbackTest1()
        {
            #region 不带返回值的

            Policy policy = Policy
                .Handle<NullReferenceException>() //故障
                .Fallback(() =>
                    Console.WriteLine("执行出错")
                , ex => {
                    Console.WriteLine(ex);
                });
            policy.Execute(() => //在策略中执行业务代码
                { 
                    //这里是可能会产生问题的业务系统代码
                    Console.WriteLine("开始任务");
                    //throw new AggregateException("Hello world excp!");
                    throw new NullReferenceException("weichuli excp!");
                    Console.WriteLine("Success! <(*￣▽￣*)/");
                }
            );

            #endregion
        }
        /// <summary>
        /// Polly 异常降级处理2
        /// </summary>
        static void PollyFallBackTest2()
        {
            #region 带返回值的

            Policy<string> policy = Policy<string>
                .Handle<NullReferenceException>(ex =>ex.Message == "custom exception") //故障
                .Fallback(() => 
                    {
                        Console.WriteLine("执行出错");
                        return @"降级后的值！\(^o^)/~";
                    }
                , ex => {
                    Console.WriteLine(ex);
                });
            var reslutStr= policy.Execute(() => //在策略中执行业务代码
                {
                    //这里是可能会产生问题的业务系统代码
                    Console.WriteLine("开始任务");
                    //throw new AggregateException("Hello world excp!");
                    throw new NullReferenceException("custom exception");
                    Console.WriteLine("Success! <(*￣▽￣*)/");
                    return "正常值";

                }
            );
            Console.WriteLine(reslutStr);

            #endregion
        }
        //------------------------------------------重试处理 Retry--------------------------------------------
        /*
         * Polly 先定义故障：异常故障，再定义故障策略 重试 Retry，然后用Policy 对象执行可能出现故障的代码
         */
        /// <summary>
        /// Polly 异常重试策略 
        /// </summary>
        static void PollyRetryTest()
        {
            try
            {
                Policy policy = Policy.Handle<Exception>()
                //.Retry() 是重试最多一次；
                //.RetryForever() 是一直重试直到成功
                //.Retry(5); // 是重试最多n次
                .WaitAndRetry(5, i => { Console.WriteLine($"time={i}"); return TimeSpan.FromSeconds(i); });
                policy.Execute(() =>
                {
                    Console.WriteLine("开始任务");
                    if (DateTime.Now.Second % 5 != 0)
                    {
                        throw new Exception("出错");
                    }
                    Console.WriteLine("Success! <(*￣▽￣*)/");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("出现未捕获异常！");
            }

        }

        /*------------------------------------------短路保护 Circuit Breaker--------------------------------------------
         * 
         * 出现N次连续错误，则把“熔断器”（保险丝）熔断，等待一段时间，等待这段时间内如果再Execute
            则直接抛出BrokenCircuitException异常，根本不会再去尝试调用业务代码。等待时间过去之后，再
            执行Execute的时候如果又错了（一次就够了），那么继续熔断一段时间，否则就恢复正常。
            这样就避免一个服务已经不可用了，还是使劲的请求给系统造成更大压力

            Polly 先定义故障：异常故障，再定义故障策略： 短路保护 Circuit Breaker，然后用Policy 对象执行可能出现故障的代码
        */
        /// <summary>
        /// Polly 异常短路保护策略 
        /// </summary>
        static void PollyCircuitBreakerTest()
        {
            Policy policy = Policy.Handle<Exception>()
                .CircuitBreaker(4, TimeSpan.FromSeconds(10));
            while (true)
            {
                try
                {
                    policy.Execute(() => {
                        Console.WriteLine("开始任务 ^_^");
                        throw new Exception("出错！");
                        Console.WriteLine("完成任务");
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Circuit Breaker出错  o(╥﹏╥)o");
                }
                System.Threading.Thread.Sleep(500);

            }

        }
        //--------------------------------------------策略封装------------------------------------------
        /*
            可以把多个ISyncPolicy合并到一起执行：
            policy3= policy1.Wrap(policy2);
            执行policy3就会把policy1、policy2封装到一起执行
            policy9=Policy.Wrap(policy1, policy2, policy3, policy4, policy5);把更多一起封装。

            下面代码实现了“出现异常则重试三次，如果还出错就FallBack”
        */
        /// <summary>
        /// Polly warp策略封装
        /// </summary>
        static void PollyWarpTest()
        {
            Policy policyRetry = Policy.Handle<Exception>()
                .Retry(3);
            Policy policyFallBack = Policy.Handle<Exception>()
                .Fallback(() => Console.WriteLine("Polly warp策略封装降级 o(￣ヘ￣o＃)"));
            //Wrap：包裹。policyRetry在里面，policyFallback裹在外面。
            //如果里面出现了故障，则把故障抛出来给外面
            Policy policy = policyFallBack.Wrap(policyRetry);
            policy.Execute(() => {
                Console.WriteLine("开始任务");
                if (DateTime.Now.Second % 5 != 0)
                {
                    throw new Exception("出错");
                }
                Console.WriteLine("Success! <(*￣▽￣*)/");
            });

        }
        //--------------------------------------------超时处理 Timeout------------------------------------------
        /*
         * Polly 先定义 超时故障，然后再定义 异常故障，和降级处理策略。然后用降级策略包裹住超时故障
         * (因为超时故障policy 如果发生超时故障 会抛TimeoutRejectedException异常，这样外层的 异常故障就可以捕捉到，然后进行降级策略处理) 
         */
        /// <summary>
        /// Polly warp超时处理
        /// </summary>
        static void PollyTimeoutTest()
        {
            Policy policyTimeout = Policy.Timeout(3, Polly.Timeout.TimeoutStrategy.Pessimistic);
            Policy policyFallBack = Policy.Handle<Polly.Timeout.TimeoutRejectedException>()
                .Fallback(() => Console.WriteLine("Timeout降级 o(￣ヘ￣o＃)"));
            Policy policy = policyFallBack.Wrap(policyTimeout);
            policy.Execute(() => {
                Console.WriteLine("开始任务");
                System.Threading.Thread.Sleep(5000);
                Console.WriteLine("Completion! <(*￣▽￣*)/");
            });
                
        }

        //--------------------------------------------Polly  的异步用法------------------------------------------

        static async Task PollyTimeoutTest1Async()
        {
            Policy policyFallback = Policy.Handle<Polly.Timeout.TimeoutRejectedException>()
                .FallbackAsync(
                    async c =>
                    {
                        Console.WriteLine("异步降级 o(￣ヘ￣o＃)");
                        await System.IO.File.ReadAllTextAsync("a.txt");

                    });

            Policy policyTimeout = Policy.TimeoutAsync(3, Polly.Timeout.TimeoutStrategy.Pessimistic, 
                async (content,timeSpan,task) => {
                    Console.WriteLine("timeout");
                });


            policyFallback =  policyFallback.WrapAsync(policyTimeout);
            await policyFallback.ExecuteAsync(async () =>
            {
                Console.WriteLine("开始任务");
                await Task.Delay(5000);
                Console.WriteLine("Completion! <(*￣▽￣*)/");
                await System.IO.File.ReadAllTextAsync("a.txt");
            });
            //Policy policy = Policy
            //    .Handle<Exception>()
            //    .FallbackAsync(async c => {
            //        Console.WriteLine("执行出错");
            //    }, async ex => {//对于没有返回值的，这个参数直接是异常
            //        Console.WriteLine(ex);
            //    });
            //policy = policy.WrapAsync(Policy.TimeoutAsync(3, TimeoutStrategy.Pessimistic,
            //async (context, timespan, task) =>
            //{
            //    Console.WriteLine("timeout");
            //}));
            //await policy.ExecuteAsync(async () => {
            //    Console.WriteLine("开始任务");
            //    await Task.Delay(5000);//注意不能用Thread.Sleep(5000);
            //    Console.WriteLine("完成任务");
            //});

        }

        static async Task PollyTimeoutTest2Async()
        {
            Policy policyTimeout = Policy.TimeoutAsync(3, TimeoutStrategy.Pessimistic,
                async (content, timeSpan, task) =>
                {
                    Console.WriteLine("timeout");
                });
            Policy<string> policyFallbak = Policy<string>.Handle<TimeoutRejectedException>().FallbackAsync(
                async (c) => {
                    Console.WriteLine("异步降级 o(￣ヘ￣o＃)");
                    await System.IO.File.ReadAllTextAsync("a.txt");
                    return "降级的值！^_^";
                });
            policyFallbak = policyFallbak.WrapAsync(policyTimeout);
            var resultStr=await policyFallbak.ExecuteAsync( 
                async()  => {
                    Console.WriteLine("开始任务");
                    await Task.Delay(5000);
                    Console.WriteLine("Completion! <(*￣▽￣*)/");
                    await System.IO.File.ReadAllTextAsync("a.txt");

                    return "正常值！";
                    }
                );
            Console.WriteLine(resultStr);
        }

        static void PollyCircuitBreakerTest2()
        {

            Policy policyTimeout = Policy.Timeout(3, Polly.Timeout.TimeoutStrategy.Pessimistic);

            Policy policy = Policy.Handle<Exception>()
                .CircuitBreaker(3, TimeSpan.FromSeconds(10));


            Policy policyFallBack = Policy.Handle<Exception>()
                .Fallback(() => Console.WriteLine("Timeout降级 o(￣ヘ￣o＃)"));
            policy = policy.Wrap(policyTimeout);
            policyFallBack = policyFallBack.Wrap(policy);
            while (true)
            {
                //try
                //{
                policyFallBack.Execute(() => {
                        Console.WriteLine("开始任务 ^_^");
                        //throw new Exception("出错！");
                        Thread.Sleep(10000);
                        Console.WriteLine("完成任务");
                    });
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine("Circuit Breaker出错  o(╥﹏╥)o");
                //}
                System.Threading.Thread.Sleep(500);

            }

        }
        static async Task PollyCircuitBreakerTest3()
        {

            Policy policyTimeout = Policy.TimeoutAsync(3, Polly.Timeout.TimeoutStrategy.Pessimistic);

            Policy policy = Policy.Handle<Exception>()
                .CircuitBreakerAsync(3, TimeSpan.FromSeconds(10));


            Policy policyFallBack = Policy.Handle<Exception>()
                .FallbackAsync(async c => { Console.WriteLine("Timeout降级 o(￣ヘ￣o＃)"); });
            policy = policy.WrapAsync(policyTimeout);
            policyFallBack = policyFallBack.WrapAsync(policy);
            while (true)
            {
                //try
                //{
                await policyFallBack.ExecuteAsync(async () => {
                    Console.WriteLine("开始任务 ^_^");
                    //throw new Exception("出错！");
                    await Task.Delay(10000);
                    Console.WriteLine("完成任务");
                });
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine("Circuit Breaker出错  o(╥﹏╥)o");
                //}
                await Task.Delay(500);

            }

        }
    }
}
