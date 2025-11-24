# مشروع Task Management API

تطبيق ويب API بسيط مبني على .NET 8.0 لإدارة المهام والبوستات.

## كيف تشغل المشروع محلياً

1. تأكد إنك عندك .NET 8.0 SDK مثبت
2. افتح Terminal في مجلد المشروع
3. شغل الأمر:
```bash
cd WebApplication1
dotnet run --project Task.PL
```

المشروع بيشتغل على `http://localhost:5000` (أو المنفذ اللي بيظهرلك)

## المميزات الأساسية

- تسجيل دخول و JWT authentication
- إدارة المهام (Todos)
- البوستات والتعليقات
- الإشعارات
