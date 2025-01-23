using System.Diagnostics;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class JobSystemExample : MonoBehaviour
{
    [SerializeField]
    private int _arrayRange = 100000;

    private void Start()
    {
        JobSytemBenchmark();
    }

    [ContextMenu("Benchmark")]
    private void JobSytemBenchmark()
    {
        long normalWatch = 0;
        long job1Watch = 0;
        long job2Watch = 0;
        long job3Watch = 0;
        long job4Watch = 0;

        int[] numbers = Enumerable.Range(0, _arrayRange).ToArray();
        bool[] result = new bool[numbers.Length];
        
        //通常の処理
        Stopwatch stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < numbers.Length; i++)
        {
            result[i] = IsPrime(numbers[i]);
        }
        stopwatch.Stop();
        normalWatch = stopwatch.ElapsedMilliseconds;

        // NativeArrayを使用してジョブの結果を格納
        using (NativeArray<int> numbersArray = new(numbers, Allocator.TempJob))
        {
            using (NativeArray<bool> resultArray = new NativeArray<bool>(numbers.Length, Allocator.TempJob))
            {
                stopwatch.Restart();
                //ジョブ1
                var job1 = new JobOne() { numbers = numbersArray, results = resultArray };
                var handle1 = job1.Schedule();

                handle1.Complete();
                stopwatch.Stop();
                job1Watch = stopwatch.ElapsedMilliseconds;

                stopwatch.Restart();

                //ジョブ2
                var job2 = new JobTwo() { numbers = numbersArray, results = resultArray };
                var handle2 = job2.Schedule();

                handle2.Complete();
                stopwatch.Stop();
                job2Watch = stopwatch.ElapsedMilliseconds;

                stopwatch.Restart();

                //ジョブ3
                var job3 = new JobThree() { numbers = numbersArray, results = resultArray };
                var handle3 = job3.Schedule(numbersArray.Length, 0);

                handle3.Complete();
                stopwatch.Stop();
                job3Watch = stopwatch.ElapsedMilliseconds;

                stopwatch.Restart();
                //ジョブ4
                var job4 = new JobFour() { numbers = numbersArray, results = resultArray };
                var handle4 = job4.Schedule(numbersArray.Length, 0);

                handle4.Complete();
                stopwatch.Stop();
                job4Watch = stopwatch.ElapsedMilliseconds;

                UnityEngine.Debug.Log($"Array Range is <color=yellow>{_arrayRange}</color>\n" +
                    $"normal : <b>{normalWatch}</b> ms\n" +
                    $"IJob : <b>{job1Watch}</b> ms\n" +
                    $"IJob Burst : <b>{job2Watch}</b> ms\n" +
                    $"IJobParallelFor : <b>{job3Watch}</b> ms\n" +
                    $"IJobParallelFor Burst : <b>{job4Watch}</b> ms");
            }
        }
    }

    static private bool IsPrime(int number)
    {
        if (number < 2) return false;
        if (number == 2) return true;
        if (number % 2 == 0) return false;

        for (int i = 3; i * i <= number; i += 2)
        {
            if (number % i == 0) return false;
        }
        return true;
    }

    private struct JobOne : IJob
    {
        public NativeArray<int> numbers;
        public NativeArray<bool> results;

        public void Execute()
        {
            for (int i = 0; i < numbers.Length; i++)
                results[i] = IsPrime(numbers[i]);
        }

        static private bool IsPrime(int number)
        {
            if (number < 2) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            for (int i = 3; i * i <= number; i += 2)
            {
                if (number % i == 0) return false;
            }
            return true;
        }
    }

    [BurstCompile]
    private struct JobTwo : IJob
    {
        public NativeArray<int> numbers;
        public NativeArray<bool> results;

        public void Execute()
        {
            for (int i = 0; i < numbers.Length; i++)
                results[i] = IsPrime(numbers[i]);
        }

        static private bool IsPrime(int number)
        {
            if (number < 2) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            for (int i = 3; i * i <= number; i += 2)
            {
                if (number % i == 0) return false;
            }
            return true;
        }
    }

    private struct JobThree : IJobParallelFor
    {
        public NativeArray<int> numbers;
        public NativeArray<bool> results;

        public void Execute(int index)
        {
            results[index] = IsPrime(numbers[index]);
        }

        static private bool IsPrime(int number)
        {
            if (number < 2) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            for (int i = 3; i * i <= number; i += 2)
            {
                if (number % i == 0) return false;
            }
            return true;
        }
    }

    [BurstCompile]
    private struct JobFour : IJobParallelFor
    {
        public NativeArray<int> numbers;
        public NativeArray<bool> results;

        public void Execute(int index)
        {
            results[index] = IsPrime(numbers[index]);
        }

        static private bool IsPrime(int number)
        {
            if (number < 2) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            for (int i = 3; i * i <= number; i += 2)
            {
                if (number % i == 0) return false;
            }
            return true;
        }
    }
}
