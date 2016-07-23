﻿using System;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using Caliburn.Micro;
using Microsoft.HockeyApp;
using ReactiveUI;
using UwCore.Services.ExceptionHandler;
using UwCore.Services.Loading;

namespace UwCore.Extensions
{
    public static class ReactiveUIExtensions
    {
        public static void AttachLoadingService(this IReactiveCommand self, string message)
        {
            self.AttachLoadingService(() => message);
        }

        public static void AttachLoadingService(this IReactiveCommand self, Func<string> message)
        {
            IDisposable currentMessage = null;
            self.IsExecuting.Subscribe(f =>
            {
                if (f)
                {
                    currentMessage = IoC.Get<ILoadingService>().Show(message());
                }
                else
                {
                    currentMessage?.Dispose();
                }
            });
        }

        public static void AttachExceptionHandler(this IReactiveCommand self)
        {
            var exceptionHandler = IoC.Get<IExceptionHandler>();
            self.ThrownExceptions.Subscribe(async e => await exceptionHandler.HandleAsync(e));
        }

        public static ObservableAsPropertyHelper<TRet> ToLoadedProperty<TObj, TRet>(this IObservable<TRet> self, TObj source, Expression<Func<TObj, TRet>> property, out ObservableAsPropertyHelper<TRet> result, TRet initialValue = default(TRet), IScheduler scheduler = null) where TObj : ReactiveObject
        {
            var res = self.ToProperty(source, property, out result, initialValue, scheduler);

            source.WhenAnyValue(property)
                .Subscribe(_ => { });

            return res;
        }

        public static void TrackEvent(this IReactiveCommand self, string eventName)
        {
            self.IsExecuting.Subscribe(f =>
            {
                if (f)
                {
                    IoC.Get<IHockeyClient>().TrackEvent(eventName);
                }
            });
        }
    }
}
