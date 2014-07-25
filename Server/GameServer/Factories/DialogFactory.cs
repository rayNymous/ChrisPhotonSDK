using System.Collections.Generic;
using GameServer.Model.Interfaces;
using GameServer.Quests;
using MayhemCommon;

namespace GameServer.Factories
{
    public class DialogFactory : IFactory
    {
        public Dictionary<int, Dialog> Dialogs;

        public DialogFactory()
        {
            Dialogs = new Dictionary<int, Dialog>();

            Dialogs.Add(0, new Dialog
            {
                Title = "Skeleton",
                Id = 0,
                Pages = new[]
                {
                    new DialogPage
                    {
                        Id = 0,
                        Text = "I'm so sick and tired of that smell! Back in my days, things did not smell so bad.",
                        IsLeftButtonEnabled = false,
                        LeftButtonText = "",
                        Links = new[]
                        {
                            new DialogLink
                            {
                                Target = 1,
                                Text = "Poor skeleton",
                                Type = DialogLinkType.Page
                            }
                        }
                    },
                    new DialogPage
                    {
                        Id = 1,
                        Text = "Whoops, I'm sorry. Where were we?",
                        IsLeftButtonEnabled = false,
                        LeftButtonText = "",
                        Links = new[]
                        {
                            new DialogLink
                            {
                                Target = 0,
                                Text = "Something about the smell?",
                                Type = DialogLinkType.Page
                            }
                        }
                    }
                }
            });
        }
    }
}