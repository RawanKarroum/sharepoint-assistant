using SPRagAPI.Models;

namespace SPRagAPI.Services;

public class FakeSharePointDocumentService : ISharePointDocumentService
{
    private static readonly List<SharePointDocument> Documents =
    [
        new SharePointDocument
        {
            Id = "doc-001",
            Title = "Employee Handbook",
            WebUrl = "https://contoso.sharepoint.com/sites/hr/Documents/Employee-Handbook.pdf",
            SiteId = "site-hr",
            LibraryName = "Documents",
            Author = "HR Team",
            LastModified = new DateTime(2025, 3, 15),
            ContentType = "pdf",
            ExtractedText = """
                Employee Handbook

                Welcome to Contoso. This handbook outlines the policies and expectations that apply to all employees.

                Working Hours
                Standard business hours are 9:00 AM to 5:00 PM, Monday through Friday. Flexible start times between 8:00 AM and 10:00 AM are available with manager approval. Employees are expected to work a minimum of 40 hours per week.

                Paid Time Off
                Full-time employees receive 20 days of paid time off (PTO) per calendar year. PTO accrues monthly and may be used for vacation, personal days, or illness. Requests must be submitted at least two weeks in advance through the HR portal, except in cases of emergency.

                Code of Conduct
                All employees must treat colleagues, customers, and partners with respect. Harassment, discrimination, and retaliation are strictly prohibited. Violations should be reported to HR or through the anonymous ethics hotline.

                Benefits Overview
                Contoso offers health, dental, and vision insurance starting on the first day of the month following your hire date. The company also provides a 401(k) plan with a 4% employer match and an annual professional development allowance of $1,500.
                """
        },
        new SharePointDocument
        {
            Id = "doc-002",
            Title = "Travel Policy",
            WebUrl = "https://contoso.sharepoint.com/sites/hr/Documents/Travel-Policy.docx",
            SiteId = "site-hr",
            LibraryName = "Documents",
            Author = "Finance Team",
            LastModified = new DateTime(2025, 1, 10),
            ContentType = "docx",
            ExtractedText = """
                Travel and Expense Policy

                Purpose
                This policy defines how employees should book, approve, and expense business travel on behalf of Contoso.

                Pre-Approval
                All domestic travel costing more than $500 and all international travel require written approval from your direct manager at least five business days before departure. Submit travel requests through the Finance portal.

                Booking Guidelines
                Employees must book flights and hotels through the company-approved travel vendor. Economy class is standard for flights under six hours. Business class requires VP approval. Preferred hotel partners offer corporate rates—use them whenever available.

                Meal Allowances
                While traveling, employees may expense meals up to $75 per day. Alcohol is not reimbursable unless entertaining clients, in which case prior manager approval is required. Keep itemized receipts for all expenses over $25.

                Reimbursement
                Submit expense reports within 30 days of returning from travel. Reports submitted after 60 days may be denied. Mileage for personal vehicle use is reimbursed at the current IRS standard rate.
                """
        },
        new SharePointDocument
        {
            Id = "doc-003",
            Title = "Remote Work Guidelines",
            WebUrl = "https://contoso.sharepoint.com/sites/hr/Documents/Remote-Work-Guidelines.pdf",
            SiteId = "site-hr",
            LibraryName = "Documents",
            Author = "HR Team",
            LastModified = new DateTime(2024, 11, 20),
            ContentType = "pdf",
            ExtractedText = """
                Remote Work Guidelines

                Overview
                Contoso supports hybrid and remote work arrangements where role requirements allow. This document describes eligibility, expectations, and equipment support for remote employees.

                Eligibility
                Employees may work remotely up to three days per week with manager approval. Fully remote arrangements require director-level approval and are reviewed annually. Roles that require on-site presence—such as lab technicians and reception staff—are not eligible for remote work.

                Home Office Setup
                Contoso provides a one-time home office stipend of $500 for eligible remote employees. This may be used toward a desk, chair, monitor, or other ergonomic equipment. Employees are responsible for maintaining a safe, distraction-free workspace and a reliable internet connection of at least 25 Mbps.

                Communication Expectations
                Remote employees must be reachable during core collaboration hours (10:00 AM – 3:00 PM local time) via Teams and email. Camera-on attendance is expected for team meetings unless otherwise noted. Managers will conduct monthly check-ins to discuss workload and wellbeing.

                Security
                All company data must be accessed through the corporate VPN. Personal devices may not be used to store or process confidential information. Report lost or stolen equipment to IT within 24 hours.
                """
        }
    ];

    public Task<IReadOnlyList<SharePointDocument>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<SharePointDocument>>(Documents);
    }

    public Task<SharePointDocument?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Documents.FirstOrDefault(d => d.Id == id));
    }
}
