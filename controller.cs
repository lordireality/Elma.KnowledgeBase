using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EleWise.ELMA.Model.Common;
using EleWise.ELMA.Model.Entities;
using EleWise.ELMA.Model.Managers;
using EleWise.ELMA.Model.Types.Settings;
using EleWise.ELMA.UI.Controllers;
using EleWise.ELMA.UI.Models;
using EleWise.ELMA.API;
using EleWise.ELMA.UI.Results;

namespace EleWise.ELMA.UI.Pages
{
	
	/// <summary>
	/// Контроллер страницы "База знаний ELMA"
	/// </summary>
	public partial class KnowledgeBaseController : PageController<KnowledgeBase.Index>
	{
		
		/// <summary>
		/// Загрузка страницы
		/// </summary>
		public override void Index_Load (PageLoadViewModel<KnowledgeBase.Index> page)
		{
			
			
		}

		/// <summary>
		/// CreateNoteItem
		/// </summary>
		/// <param name="page"></param>
		public virtual void CreateNoteItem (PageViewModel<KnowledgeBase.Index> page)
		{
			
			if(page.Context.CreateCategory != null && !String.IsNullOrEmpty(page.Context.CreateName))
			{
				var newInstance = EntityManager<EleWise.ELMA.ConfigurationModel.ElmaKnowledgeBase, long>.Instance.Create ();
				newInstance.Category = page.Context.CreateCategory;
				newInstance.Name = page.Context.CreateName;
				newInstance.Text = page.Context.CreateText;
				foreach(var element in page.Context.CreateRelatedNotes)
				{
					var related = new EleWise.ELMA.ConfigurationModel.ElmaKnowledgeBase_RelatedNotes();
					related.Name = element.Name;
					related.Link = element.Link;
					newInstance.RelatedNotes.Add(related);
				}
				newInstance.Author = PublicAPI.Portal.Security.User.GetCurrentUser();
				newInstance.CreationDate = DateTime.Now;
				newInstance.Save();
				page.Context.CreateCategory = null;
				page.Context.CreateName = "";
				page.Context.CreateText = null;
				page.Context.CreateRelatedNotes.Clear();
				page.Form.Notifier.Information("Запись создана!");
			} else
			{
				page.Form.Notifier.Error("Ошибка, не заполнены все поля!");
			}

		}

		/// <summary>
		/// FindNotes
		/// </summary>
		/// <param name="page"></param>
		public virtual void FindNotes (PageViewModel<KnowledgeBase.Index> page)
		{
			page.Context.FindedNotes.Clear();
			string eqlString = "Id <> 0";
			if(!String.IsNullOrEmpty(page.Context.SearchString))
			{
				eqlString = eqlString + " and Name like '%"+page.Context.SearchString+"%'";
			}
			if(page.Context.SearchCategory != null)
			{
				eqlString = eqlString + " and Category = DropDownItem('"+page.Context.SearchCategory.Value+"')";
			}
			if(page.Context.SearchId.HasValue)
			{
				eqlString = eqlString + " and Id = "+ page.Context.SearchId.ToString();
			}
			var NotesItems = PublicAPI.Objects.UserObjects.UserElmaKnowledgeBase.Find(eqlString).OrderByDescending(c=>c.Name);
			foreach(var item in NotesItems)
			{
				var blockItem = new KnowledgeBase.Index_FindedNotes();
				blockItem.Item = item;
				page.Context.FindedNotes.Add(blockItem);
			}
			
			
		}
	}
}
