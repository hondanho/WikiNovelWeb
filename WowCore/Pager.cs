
using System;
using System.Collections.Generic;
using System.Linq;


namespace WowCore
{
  public class Pager
  {
    public const int Page_Size = 15;
    public const int Max_Pages = 10;

    public Pager(int totalItems, int currentPage = 1, int pageSize = 10, int maxPages = 10)
    {
      int num1 = (int) Math.Ceiling((Decimal) totalItems / (Decimal) pageSize);
      if (currentPage < 1)
        currentPage = 1;
      else if (currentPage > num1)
        currentPage = num1;
      int start;
      int num2;
      if (num1 <= maxPages)
      {
        start = 1;
        num2 = num1;
      }
      else
      {
        int num3 = (int) Math.Floor((Decimal) maxPages / 2M);
        int num4 = (int) Math.Ceiling((Decimal) maxPages / 2M) - 1;
        if (currentPage <= num3)
        {
          start = 1;
          num2 = maxPages;
        }
        else if (currentPage + num4 >= num1)
        {
          start = num1 - maxPages + 1;
          num2 = num1;
        }
        else
        {
          start = currentPage - num3;
          num2 = currentPage + num4;
        }
      }
      int num5 = (currentPage - 1) * pageSize;
      int num6 = Math.Min(num5 + pageSize - 1, totalItems - 1);
      IEnumerable<int> ints = Enumerable.Range(start, num2 + 1 - start);
      this.TotalItems = totalItems;
      this.CurrentPage = currentPage;
      this.PageSize = pageSize;
      this.TotalPages = num1;
      this.StartPage = start;
      this.EndPage = num2;
      this.StartIndex = num5;
      this.EndIndex = num6;
      this.Pages = ints;
    }

    public int TotalItems { get; private set; }

    public int CurrentPage { get; private set; }

    public int PageSize { get; private set; }

    public int TotalPages { get; private set; }

    public int StartPage { get; private set; }

    public int EndPage { get; private set; }

    public int StartIndex { get; private set; }

    public int EndIndex { get; private set; }

    public int PrevPage => this.CurrentPage - 1;

    public int NextPage => this.CurrentPage == this.TotalPages ? -1 : this.CurrentPage + 1;

    public IEnumerable<int> Pages { get; private set; }
  }
}
