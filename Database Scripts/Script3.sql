USE [NubiDB]
GO
SET IDENTITY_INSERT [dbo].[RelationshipType] ON 
GO
INSERT [dbo].[RelationshipType] ([Id], [RelationEn], [RelationAr]) VALUES (1, N'Father', N'اب')
GO
INSERT [dbo].[RelationshipType] ([Id], [RelationEn], [RelationAr]) VALUES (2, N'Mother', N'ام')
GO
INSERT [dbo].[RelationshipType] ([Id], [RelationEn], [RelationAr]) VALUES (3, N'Brother', N'اخ')
GO
INSERT [dbo].[RelationshipType] ([Id], [RelationEn], [RelationAr]) VALUES (4, N'Sister', N'اخت')
GO
INSERT [dbo].[RelationshipType] ([Id], [RelationEn], [RelationAr]) VALUES (5, N'Husband', N'زوج')
GO
INSERT [dbo].[RelationshipType] ([Id], [RelationEn], [RelationAr]) VALUES (6, N'Wife', N'زوجة')
GO
INSERT [dbo].[RelationshipType] ([Id], [RelationEn], [RelationAr]) VALUES (7, N'Other', N'اخرى')
GO
SET IDENTITY_INSERT [dbo].[RelationshipType] OFF
GO
