# Báo cáo Debug các vấn đề Financial

## Các vấn đề đã phát hiện:

### 1. **Null Reference trong GetDebtReportAsync** (FinancialController.cs:278-285)
**Vấn đề**: Nếu `contract == null`, code vẫn cố truy cập `contract.MaNguoiThue` và `contract.MaPhong`
```csharp
var contract = await _contractRepository.GetByIdAsync(debt.MaHopDong ?? 0);
if (contract == null) continue; // ✅ Đã có check

var room = await _roomRepository.GetByIdAsync(contract.MaPhong); // ✅ OK vì đã check null
var tenant = await _tenantRepository.GetByIdAsync(contract.MaNguoiThue); // ✅ OK
```

### 2. **Null Reference trong GetTransactionHistoryAsync** (FinancialController.cs:581-582)
**Vấn đề**: Nếu `contract == null`, code vẫn cố truy cập `contract.MaNguoiThue`
```csharp
var contract = await _contractRepository.GetByIdAsync(transaction.MaHopDong ?? 0);
var tenant = await _tenantRepository.GetByIdAsync(contract.MaNguoiThue); // ❌ Có thể null reference
```

### 3. **Database Connection Timeout**
**Đã sửa**: Thêm `ConnectionTimeout=10` và `DefaultCommandTimeout=30` vào connection string

### 4. **GetAllAsync không có error handling**
**Vấn đề**: Nếu database connection fail, sẽ throw exception và crash app
**Giải pháp**: Cần thêm try-catch và fallback

### 5. **GetFinancialStatsAsync có thể null reference**
**Vấn đề**: Nếu `NgayThanhToan` là NULL, query `YEAR(NgayThanhToan)` sẽ fail
**Giải pháp**: Cần filter NULL trước khi query

## Các vấn đề cần sửa ngay:

1. ✅ **GetTransactionHistoryAsync** - Thêm null check cho contract
2. ✅ **GetFinancialStatsAsync** - Filter NULL NgayThanhToan
3. ✅ **GetAllAsync** - Thêm error handling
4. ✅ **GetDebtReportAsync** - Đã OK, nhưng cần verify

