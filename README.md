# Almakaber API - المقابر (Backend)

[![Live Demo](https://img.shields.io/badge/Live_Demo-View_Website-success?style=for-the-badge&logo=vercel)](https://almakaber.vercel.app/)
[![Frontend Repo](https://img.shields.io/badge/Frontend-Angular_Repository-informational?style=for-the-badge&logo=angular)](https://github.com/Hassan1Abdallah/almakaber-frontend.git)

هذا هو المستودع الخاص بـ **Backend** لمنصة **المقابر**، وهي منصة خدمية مفتوحة المصدر تهدف إلى إدارة بيانات المتوفين، وتنظيم أماكن الدفن استدلاليًا لتسهيل الوصول إليها، وتقديم ساحة للأدعية المستمرة للمتوفين (مسبحة إلكترونية).

**رابط الديمو (Live Demo):** [almakaber.vercel.app](https://almakaber.vercel.app/)  
🔗 ** (Frontend):** [Repo](https://github.com/Hassan1Abdallah/almakaber-frontend.git)

---

# التقنيات المستخدمة (Tech Stack)

- **Framework:** ASP.NET Core Web API
- **Database:** SQL Server (Entity Framework Core)
- **Authentication:** JWT (JSON Web Tokens)
- **Testing:** xUnit (`Almakaber.Tests`)
- **CI/CD:** GitHub Actions ( `.github/workflows`)

---

# كيفية تشغيل المشروع محليًا (Local Setup)

## 1. المتطلبات الأساسية

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server

## 2. استنساخ المشروع

```bash
git clone https://github.com/YourUsername/Almakaber-Backend.git
cd Almakaber-Backend
```

## 3. إعداد ملف الإعدادات

قم بإنشاء ملف:

```text
appsettings.json
```

ثم قم بإضافة إعداداتك الخاصة.

عدّل قيمة:

```json
DefaultConnection
```

لتتوافق مع إعدادات SQL Server لديك.

## 4. إنشاء قاعدة البيانات

نفذ الأمر التالي:

```bash
dotnet ef database update
```

---

# تشغيل المشروع

بعد الانتهاء من الإعدادات، شغّل المشروع:

```bash
dotnet run --project Almakaber
```

بعد التشغيل يمكنك فتح **Swagger UI** واختبار جميع الـ Endpoints.

---


# الدعم والتواصل

إذا واجهت أي مشكلة أثناء تشغيل المشروع محليًا، أو احتجت إلى مساعدة في إعداد قاعدة البيانات أو ربط الـ Backend بالـ Frontend، فلا تتردد في التواصل معي.

**Email:** `hasan.dev.inf.com`

**WhatsApp:** `+201017209315`
