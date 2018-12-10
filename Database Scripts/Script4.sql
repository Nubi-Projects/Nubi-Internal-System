USE [NubiDB]
GO

ALTER TABLE [dbo].[EmergencyContact]  WITH CHECK ADD  CONSTRAINT [FK_EmergencyContact_Employee] FOREIGN KEY([EmpNo])
REFERENCES [dbo].[Employee] ([Id])
GO
ALTER TABLE [dbo].[Attachment] CHECK CONSTRAINT [FK_Attachment_TypesOfAttachment]
GO
