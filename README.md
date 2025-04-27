# Medical Appointment System

一個使用 .NET Core 後端和 React 前端的綜合醫療預約管理系統。

## 專案結構

### 後端 (.NET Core)
- **Controllers**: API 端點處理請求
  - `PatientController`: 處理病患相關操作
  - `DoctorController`: 處理醫生相關操作
  - `AppointmentController`: 處理預約管理
  - `AuthController`: 處理認證和授權

- **Services**: 業務邏輯實現
  - `PatientService`: 病患資料和預約管理
  - `DoctorService`: 醫生排班和可用時間管理
  - `AppointmentService`: 預約預訂和管理
  - `AuthService`: 用戶認證和授權

- **Models**: 資料模型和 DTOs
  - `Patient`: 病患資訊
  - `Doctor`: 醫生資訊
  - `Appointment`: 預約詳情
  - `User`: 用戶認證
  - `DTOs`: API 請求/響應的資料傳輸對象

- **Data**: 資料庫上下文和配置
  - `ApplicationDbContext`: Entity Framework Core 上下文
  - 資料庫遷移

- **Middleware**: 自定義中介軟體組件
  - 認證中介軟體
  - 錯誤處理中介軟體

- **Utils**: 工具類和輔助類
  - `ServiceResult`: 標準化服務響應
  - `Constants`: 應用程式常量

### 前端 (React + Vite)
- **Components**: 可重用的 UI 組件
  - 病患組件
  - 醫生組件
  - 預約組件
  - 認證組件

- **Pages**: 主要應用頁面
  - 病患儀表板
  - 醫生儀表板
  - 預約預訂
  - 個人資料管理

- **Services**: API 整合
  - 病患服務
  - 醫生服務
  - 預約服務
  - 認證服務

- **Utils**: 輔助函數和常量
  - API 客戶端
  - 認證輔助工具
  - 表單驗證

## 功能特點

- **用戶認證**
  - 病患和醫生註冊
  - 登入/登出功能
  - 基於角色的訪問控制

- **預約管理**
  - 安排預約
  - 查看預約歷史
  - 取消預約
  - 即時可用性檢查

- **個人資料管理**
  - 病患資料創建和更新
  - 醫生資料管理
  - 醫療歷史追蹤

- **醫生排班**
  - 設置工作時間
  - 管理可用時間
  - 查看預約日曆

## 開始使用

### 環境需求
- .NET Core SDK
- Node.js 和 npm
- SQL Server

### 後端設置
1. 進入 Backend 目錄
2. 在 `appsettings.json` 中更新連接字串
3. 運行資料庫遷移：
   ```bash
   dotnet ef database update
   ```
4. 啟動伺服器：
   ```bash
   dotnet run
   ```

### 前端設置
1. 進入 frontend 目錄
2. 安裝依賴：
   ```bash
   npm install
   ```
3. 啟動開發伺服器：
   ```bash
   npm run dev
   ```

## API 文檔

當後端伺服器運行時，可以在 `/swagger` 路徑查看 API 文檔。

## 貢獻指南

1. Fork 本專案
2. 創建你的功能分支
3. 提交你的更改
4. 推送到分支
5. 創建新的 Pull Request

## 授權

本專案採用 MIT 授權條款。
