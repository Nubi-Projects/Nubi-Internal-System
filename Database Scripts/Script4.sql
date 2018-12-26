USE [NubiDB]
GO

ALTER TABLE [dbo].[EmergencyContact]  WITH CHECK ADD  CONSTRAINT [FK_EmergencyContact_Employee] FOREIGN KEY([EmpNo])
REFERENCES [dbo].[Employee] ([Id])
GO
ALTER TABLE [dbo].[Attachment]  WITH CHECK ADD  CONSTRAINT [FK_Attachment_TypesOfAttachment] FOREIGN KEY([TypeOfAttachmentNo])
REFERENCES [dbo].[TypesOfAttachment] ([Id])
GO
ALTER TABLE [dbo].[Attachment] CHECK CONSTRAINT [FK_Attachment_TypesOfAttachment]
GO
