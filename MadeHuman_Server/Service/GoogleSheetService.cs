using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using MadeHuman_Server.Data;
using Microsoft.EntityFrameworkCore;

namespace MadeHuman_Server.Service
{
    public class GoogleSheetService
    {
        private readonly SheetsService _sheetsService;
        private readonly ApplicationDbContext _context;
        private readonly string _spreadsheetId = "1hBTMjqXyn6eTvfG9Jw2hoWjKhh1z7RcTpHZvQeES95M";
        private readonly string _sheetName = "PartTimeAssignments"; // 👈 hoặc tên tab bạn đang dùng nếu khác


        public GoogleSheetService(ApplicationDbContext context)
        {
            _context = context;

            // 🔹 Đoạn bạn hỏi: để trong constructor
            GoogleCredential credential;
            using (var stream = new FileStream("Data/credentials.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(SheetsService.Scope.Spreadsheets);
            }

            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "MadeHuman Google Sheet Sync"
            });
        }

        public async Task SyncPartTimeAssignmentsAsync()
        {
            var assignments = await _context.PartTimeAssignment.OrderBy(x => x.WorkDate).ToListAsync();

            var data = new List<IList<object>>
        {
            new List<object> { "Id", "PartTimeId", "WorkDate", "TaskType", "ShiftCode", "IsConfirmed", "Note" }
        };

            foreach (var item in assignments)
            {
                data.Add(new List<object>
            {
                item.Id,
                item.PartTimeId,
                item.WorkDate.ToString("yyyy-MM-dd"),
                item.TaskType.ToString(),
                item.ShiftCode ?? "",
                item.IsConfirmed ? "Yes" : "No",
                item.Note ?? ""
            });
            }

            var valueRange = new ValueRange { Values = data };

            // clear cũ
            await _sheetsService.Spreadsheets.Values.Clear(
                new ClearValuesRequest(), _spreadsheetId, $"{_sheetName}!A1:G").ExecuteAsync();

            var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, _spreadsheetId, $"{_sheetName}!A1");
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            await updateRequest.ExecuteAsync();

        }
    }
}