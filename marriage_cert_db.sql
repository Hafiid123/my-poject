-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: May 11, 2026 at 04:51 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.0.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `marriage_cert_db`
--

-- --------------------------------------------------------

--
-- Table structure for table `admin_notification_read_states`
--

CREATE TABLE `admin_notification_read_states` (
  `UserId` int(11) NOT NULL,
  `LastReadAt` datetime(6) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `admin_notification_read_states`
--

INSERT INTO `admin_notification_read_states` (`UserId`, `LastReadAt`) VALUES
(1, '2026-05-08 13:24:17.124579');

-- --------------------------------------------------------

--
-- Table structure for table `certificates`
--

CREATE TABLE `certificates` (
  `Id` int(11) NOT NULL,
  `ApplicationId` int(11) NOT NULL,
  `CertificateFile` varchar(500) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `certificates`
--

INSERT INTO `certificates` (`Id`, `ApplicationId`, `CertificateFile`) VALUES
(4, 4, 'uploads/certificates/4_e1021174c65e4d908ab0b908d639d098.pdf'),
(6, 5, 'uploads/certificates/5_86c2e2bcb8784e7fa7aaa69fc278fe94.pdf'),
(7, 1, 'uploads/certificates/1_98222013fa3f4104bc034e2c47327480.pdf'),
(8, 6, 'uploads/certificates/6_c10f06e72dbd4323ac17d91e3d5b298b.pdf'),
(9, 8, 'uploads/certificates/8_2dabd7f65f1640c99764ec5bd6a8fa41.pdf');

-- --------------------------------------------------------

--
-- Table structure for table `documents`
--

CREATE TABLE `documents` (
  `Id` int(11) NOT NULL,
  `ApplicationId` int(11) NOT NULL,
  `FilePath` varchar(500) NOT NULL,
  `Category` varchar(50) NOT NULL DEFAULT 'Supporting'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `documents`
--

INSERT INTO `documents` (`Id`, `ApplicationId`, `FilePath`, `Category`) VALUES
(1, 1, 'uploads/documents/ecd86821ee6743a5848075de7ce27d42.png', 'Supporting'),
(2, 10, 'uploads/documents/2df4a87e5ebc49cb87af771cf4d63859.png', 'HusbandIdentityDocument'),
(3, 10, 'uploads/documents/d8c10d3e79af47f997870c1560fdd60c.jpg', 'WifeIdentityDocument'),
(4, 10, 'uploads/documents/955f527105ee49398e5d0c2cef77dd7f.pdf', 'Witness1IdentityDocument'),
(5, 10, 'uploads/documents/f6f51b1af889454f9dd0492f95a60d8e.jpg', 'Witness2IdentityDocument'),
(6, 10, 'uploads/documents/b8eb2fe46f644a3e8791289f1601259d.webp', 'HusbandPassportPhoto'),
(7, 10, 'uploads/documents/1f1d1090e6ee47fb9f0e61f7da9ace41.webp', 'WifePassportPhoto'),
(8, 11, 'uploads/documents/e2b124c372224aaf98a608d873863658.jpg', 'HusbandIdentityDocument'),
(9, 11, 'uploads/documents/020f2ad6885243049775f6e7ca7ff8ef.png', 'WifeIdentityDocument'),
(10, 11, 'uploads/documents/fadbeb2071564dc5b0b9d167581c4e3f.png', 'Witness1IdentityDocument'),
(11, 11, 'uploads/documents/3c1bcaef51724baaa57d7483091dc8f6.png', 'Witness2IdentityDocument'),
(12, 11, 'uploads/documents/f9aba754972b4224872a21d09d6ce0c5.jpg', 'HusbandPassportPhoto'),
(13, 11, 'uploads/documents/4deb7e380a6e41b29c79985c58b3f29c.png', 'WifePassportPhoto');

-- --------------------------------------------------------

--
-- Table structure for table `fees`
--

CREATE TABLE `fees` (
  `Id` int(11) NOT NULL,
  `ServiceName` varchar(200) NOT NULL,
  `Amount` decimal(18,2) NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `Currency` varchar(10) NOT NULL DEFAULT 'USD',
  `CreatedAt` datetime(6) NOT NULL DEFAULT current_timestamp(6)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `fees`
--

INSERT INTO `fees` (`Id`, `ServiceName`, `Amount`, `IsActive`, `Currency`, `CreatedAt`) VALUES
(1, 'Marriage Application', 15.00, 1, 'USD', '2026-05-02 09:15:25.238786'),
(2, 'certificate', 15.10, 1, 'USD', '2026-05-02 09:15:25.238786');

-- --------------------------------------------------------

--
-- Table structure for table `marriage_applications`
--

CREATE TABLE `marriage_applications` (
  `Id` int(11) NOT NULL,
  `UserId` int(11) NOT NULL,
  `HusbandName` varchar(200) NOT NULL,
  `HusbandDob` datetime(6) NOT NULL,
  `HusbandIdNumber` varchar(100) NOT NULL,
  `HusbandContactNumber` varchar(50) NOT NULL,
  `HusbandAddress` varchar(500) NOT NULL,
  `WifeName` varchar(200) NOT NULL,
  `WifeDob` datetime(6) NOT NULL,
  `WifeIdNumber` varchar(100) NOT NULL,
  `WifeContactNumber` varchar(50) NOT NULL,
  `WifeAddress` varchar(500) NOT NULL,
  `MarriageDate` datetime(6) NOT NULL,
  `MarriageLocation` varchar(300) NOT NULL,
  `Status` varchar(30) NOT NULL,
  `SubmissionDate` datetime(6) NOT NULL,
  `Remarks` varchar(2000) DEFAULT NULL,
  `DecisionDate` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `marriage_applications`
--

INSERT INTO `marriage_applications` (`Id`, `UserId`, `HusbandName`, `HusbandDob`, `HusbandIdNumber`, `HusbandContactNumber`, `HusbandAddress`, `WifeName`, `WifeDob`, `WifeIdNumber`, `WifeContactNumber`, `WifeAddress`, `MarriageDate`, `MarriageLocation`, `Status`, `SubmissionDate`, `Remarks`, `DecisionDate`) VALUES
(1, 2, 'moha hh', '1996-04-05 00:00:00.000000', '434354553', '61225256', 'Hodan', 'oha hhy', '1998-04-05 00:00:00.000000', '545455', '463643746', 'Hodan', '2026-04-05 00:00:00.000000', 'Hodan taleex', 'Approved', '2026-04-05 05:28:28.195347', NULL, '2026-04-05 05:28:28.195347'),
(4, 2, 'ahdjsahdask', '1996-04-05 00:00:00.000000', '545744', '374837483', 'dhdjsfsj', 'sdfjhsdgfsdjh', '1998-04-05 00:00:00.000000', '65456748', '36487423', 'dsjhkfdkj', '2026-04-05 00:00:00.000000', 'hfdfhfdj', 'Rejected', '2026-04-05 07:29:55.469376', 'fgfgfgdf', '2026-04-05 07:29:55.469376'),
(5, 2, 'Maxmed Ahmed Ali', '1996-04-05 00:00:00.000000', '344343', '54748', 'Kaaran', 'Xalimo Xsan Axmed', '1998-04-05 00:00:00.000000', '564747', '8674393', 'Yaqshiid', '2026-04-05 00:00:00.000000', 'Banaadir', 'Approved', '2026-04-05 07:38:32.210293', NULL, '2026-04-05 07:38:32.210293'),
(6, 7, 'dshdjshak', '1996-05-02 00:00:00.000000', '6434637', '7463287468', 'hjsdhdj', 'shdjsadjha', '1998-05-02 00:00:00.000000', '37438242', '3779328', 'hfdkjfsd', '2026-05-02 00:00:00.000000', 'hdksajhdak', 'Approved', '2026-05-02 05:23:35.857410', NULL, '2026-05-02 05:23:35.857410'),
(8, 7, 'dshdshdais', '1996-05-02 00:00:00.000000', '454545', '656644', 'fdgfdgdf', 'fgfgfdgfdg', '1998-05-02 00:00:00.000000', '5554', '6565', 'gfgfgdfgf', '2026-05-02 00:00:00.000000', 'fgf', 'Approved', '2026-05-02 06:43:34.068857', NULL, '2026-05-07 17:38:35.862941'),
(9, 7, 'eiuei', '1996-05-02 00:00:00.000000', '45454', '5665', 'hkjd', '555345', '1998-05-02 00:00:00.000000', '45353', '5354353', 'hfkdfhsdk', '2026-05-02 00:00:00.000000', 'hlkslfs', 'Pending Payment', '2026-05-02 10:58:57.201914', NULL, NULL),
(10, 7, 'hdhdj', '1996-05-03 00:00:00.000000', '47324', '7448', 'ffjhdfkj', 'dhhhdsfksdh', '1998-05-03 00:00:00.000000', '7643874', '36238', 'hdgfdsjh', '2026-05-03 00:00:00.000000', 'djdsahdas', 'Pending Payment', '2026-05-03 04:17:01.739847', NULL, NULL),
(11, 7, 'cali', '1996-05-03 00:00:00.000000', '535345', '54445', 'gffgsfg', 'fjhkfksdlfsdj', '1998-05-03 00:00:00.000000', '735876485', '4759345073', 'kfhfdhfdj', '2026-05-03 00:00:00.000000', 'meshadjsd', 'Pending Payment', '2026-05-03 07:41:13.443572', NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `marriage_witnesses`
--

CREATE TABLE `marriage_witnesses` (
  `Id` int(11) NOT NULL,
  `ApplicationId` int(11) NOT NULL,
  `SortOrder` tinyint(3) UNSIGNED NOT NULL,
  `FullName` varchar(200) NOT NULL,
  `DateOfBirth` datetime(6) NOT NULL,
  `IdNumber` varchar(100) NOT NULL,
  `ContactNumber` varchar(50) NOT NULL,
  `Address` varchar(500) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `marriage_witnesses`
--

INSERT INTO `marriage_witnesses` (`Id`, `ApplicationId`, `SortOrder`, `FullName`, `DateOfBirth`, `IdNumber`, `ContactNumber`, `Address`) VALUES
(3, 4, 1, 'xdjhfhsdfksd', '1991-04-05 00:00:00.000000', '456645', '478563478', 'dhjfdshjs'),
(4, 4, 2, 'sejhfakjdhas', '1986-04-05 00:00:00.000000', '3767832', '74538', 'skfhdsfjksdh'),
(5, 5, 1, 'farax cali', '1991-04-05 00:00:00.000000', '5737', '61374848', 'yaqshiid'),
(6, 5, 2, 'yuusuf ahmed', '1986-04-05 00:00:00.000000', '75778', '613333', 'yaqshiid'),
(7, 6, 1, 'fhdsfksf', '1991-05-02 00:00:00.000000', '473984', '7236217', 'sdhgasjhd'),
(8, 6, 2, 'djkhkjdhska', '1986-05-02 00:00:00.000000', '37473242', '386483274', 'shdjksadhas'),
(11, 8, 1, 'fggdgfgdf', '1991-05-02 00:00:00.000000', '535', '53353', 'dggdgdf'),
(12, 8, 2, 'ggdgdgfdg', '1986-05-02 00:00:00.000000', '3553', '442', 'dsfdsfsd'),
(13, 9, 1, 'hfkjfhkdjfds', '1991-05-02 00:00:00.000000', '65654', '654645', 'jfkf;gkja'),
(14, 9, 2, 'kdkjfhakal', '1986-05-02 00:00:00.000000', '564564', '56564', 'dfjlkdfks'),
(15, 10, 1, 'ydfgjh', '1991-05-03 00:00:00.000000', '3637826', '62347', 'hdghjsa'),
(16, 10, 2, 'fgjsgds', '1986-05-03 00:00:00.000000', '63265267', '3553767', 'hsjdja'),
(17, 11, 1, 'dshfshfjsdfjhs', '1991-05-03 00:00:00.000000', '475845', '454378567348', 'sfhdsjfhdsjf'),
(18, 11, 2, 'hskjhkfhdkjfhdsj', '1986-05-03 00:00:00.000000', '347545748', '7657434', 'hjkjkdgjhjk');

-- --------------------------------------------------------

--
-- Table structure for table `payments`
--

CREATE TABLE `payments` (
  `Id` int(11) NOT NULL,
  `ApplicationId` int(11) NOT NULL,
  `Amount` decimal(18,2) NOT NULL,
  `PaymentStatus` varchar(30) NOT NULL,
  `PaymentDate` datetime(6) DEFAULT NULL,
  `ReceiptImage` varchar(500) DEFAULT NULL,
  `UserId` int(11) NOT NULL,
  `FeeId` int(11) NOT NULL,
  `SenderPhone` varchar(50) DEFAULT NULL,
  `TransactionNumber` varchar(100) DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `payments`
--

INSERT INTO `payments` (`Id`, `ApplicationId`, `Amount`, `PaymentStatus`, `PaymentDate`, `ReceiptImage`, `UserId`, `FeeId`, `SenderPhone`, `TransactionNumber`, `CreatedAt`) VALUES
(1, 1, 10.00, 'Pending', NULL, 'uploads/receipts/1_b3f6fda2276141f293e4ca1a61c225ee.png', 2, 1, NULL, NULL, '2026-05-02 06:15:25.452084'),
(2, 4, 10.00, 'Pending', NULL, NULL, 2, 1, NULL, NULL, '2026-05-02 06:15:25.452084'),
(3, 5, 10.00, 'Pending', NULL, NULL, 2, 1, NULL, NULL, '2026-05-02 06:15:25.452084'),
(4, 6, 10.00, 'Approved', '2026-05-02 05:25:56.036039', 'uploads/receipts/6_d10a22a4b9084299986090c0db3ab32d.png', 7, 1, NULL, NULL, '2026-05-02 05:25:56.036039'),
(6, 8, 15.00, 'Approved', '2026-05-07 17:37:04.657800', 'uploads/payments/8_4275c5c1697e44db9a32f1c2212d79d4.png', 7, 1, '619983958', '6566573737', '2026-05-02 06:43:34.161196'),
(7, 9, 15.00, 'Pending', NULL, NULL, 7, 2, NULL, NULL, '2026-05-02 10:58:57.369523'),
(8, 10, 15.00, 'Pending', NULL, NULL, 7, 1, NULL, NULL, '2026-05-03 04:17:02.066876'),
(9, 11, 15.10, 'Pending', NULL, NULL, 7, 2, NULL, NULL, '2026-05-03 07:41:13.748258');

-- --------------------------------------------------------

--
-- Table structure for table `permissions`
--

CREATE TABLE `permissions` (
  `Id` int(11) NOT NULL,
  `Name` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `permissions`
--

INSERT INTO `permissions` (`Id`, `Name`) VALUES
(3, 'ApproveApplication'),
(10, 'ApproveApplications'),
(1, 'CreateApplication'),
(4, 'IssueCertificate'),
(7, 'ManageFees'),
(8, 'ManagePayments'),
(6, 'ManageRoles'),
(5, 'ManageUsers'),
(11, 'RejectApplications'),
(2, 'ViewApplication'),
(9, 'ViewDashboard');

-- --------------------------------------------------------

--
-- Table structure for table `roles`
--

CREATE TABLE `roles` (
  `Id` int(11) NOT NULL,
  `Name` varchar(100) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL DEFAULT current_timestamp(6)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `roles`
--

INSERT INTO `roles` (`Id`, `Name`, `CreatedAt`) VALUES
(1, 'Rejister', '2026-05-07 14:02:59.118116'),
(2, 'Admin', '2026-05-07 14:02:59.133364'),
(3, 'User', '2026-05-07 12:15:53.156583');

-- --------------------------------------------------------

--
-- Table structure for table `role_permissions`
--

CREATE TABLE `role_permissions` (
  `RoleId` int(11) NOT NULL,
  `PermissionId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `role_permissions`
--

INSERT INTO `role_permissions` (`RoleId`, `PermissionId`) VALUES
(1, 1),
(1, 2),
(1, 3),
(1, 4),
(1, 8),
(1, 9),
(1, 10),
(1, 11),
(2, 1),
(2, 2),
(2, 3),
(2, 4),
(2, 5),
(2, 6),
(2, 7),
(2, 8),
(2, 9),
(2, 10),
(2, 11);

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `Id` int(11) NOT NULL,
  `Name` varchar(200) NOT NULL,
  `Email` varchar(256) NOT NULL,
  `Password` varchar(500) NOT NULL,
  `Role` varchar(50) NOT NULL,
  `PaymentStatus` varchar(30) NOT NULL DEFAULT 'Unpaid',
  `RoleId` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`Id`, `Name`, `Email`, `Password`, `Role`, `PaymentStatus`, `RoleId`) VALUES
(1, 'System Administrator', 'admin@system.local', '$2a$11$DHJI2HRt4i0XCz8Nq0214.dOunhx2LG7P0ObaEsuviwA7MoKj.Ls6', 'Admin', 'Unpaid', 2),
(2, 'cali', 'cali@gmail.com', '$2a$11$KfPlOFqDGcfwkX8iMeVt1e1kFuDkRGTxS4UlcrQqgc5t8wsqlcJpe', 'User', 'Unpaid', 1),
(6, 'ahmed', 'ahmed@gmail.com', '$2a$11$gz/SDZG5L3/R5XsnjJgZw.tWvqwmPARlcj4PqUt9HXL8PF.EnwSO2', 'User', 'Unpaid', 1),
(7, 'moha abdi ali', 'moha12@gmail.com', '$2a$11$EAnldWG6OX5bNxQy3yJpAOBEqC0zD9BKjTdHLRUnKWsZFXypw.4NS', 'User', 'Paid', 1),
(10, 'xalimo', 'xalimo@gmail.com', '$2a$11$r/K/V6g.kfY6iVfmohye5.GGSWwxIEKTxF/bsZ0LQbO6xdX9nsaE6', 'Staff', 'Unpaid', 1);

-- --------------------------------------------------------

--
-- Table structure for table `__efmigrationshistory`
--

CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `__efmigrationshistory`
--

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES
('20260405051534_InitialCreate', '8.0.11'),
('20260405064655_AddMarriageWitnesses', '8.0.11'),
('20260502051853_AddFeesAndPayments', '8.0.11'),
('20260502061052_ManualPaymentGatewayFlow', '8.0.11'),
('20260503104500_AddDocumentCategory', '8.0.11'),
('20260507103000_AddRbacCore', '8.0.11'),
('20260507120000_AddAdminPermissionsAndAccess', '8.0.11'),
('20260507141500_AddApplicationDecisionDate', '8.0.11'),
('20260507153000_AddAdminNotificationReadState', '8.0.11');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `admin_notification_read_states`
--
ALTER TABLE `admin_notification_read_states`
  ADD PRIMARY KEY (`UserId`);

--
-- Indexes for table `certificates`
--
ALTER TABLE `certificates`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `IX_certificates_ApplicationId` (`ApplicationId`);

--
-- Indexes for table `documents`
--
ALTER TABLE `documents`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_documents_ApplicationId` (`ApplicationId`);

--
-- Indexes for table `fees`
--
ALTER TABLE `fees`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `marriage_applications`
--
ALTER TABLE `marriage_applications`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_marriage_applications_UserId` (`UserId`);

--
-- Indexes for table `marriage_witnesses`
--
ALTER TABLE `marriage_witnesses`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `IX_marriage_witnesses_ApplicationId_SortOrder` (`ApplicationId`,`SortOrder`);

--
-- Indexes for table `payments`
--
ALTER TABLE `payments`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `IX_payments_ApplicationId` (`ApplicationId`),
  ADD KEY `IX_payments_UserId` (`UserId`),
  ADD KEY `IX_payments_FeeId` (`FeeId`);

--
-- Indexes for table `permissions`
--
ALTER TABLE `permissions`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `UX_permissions_Name` (`Name`);

--
-- Indexes for table `roles`
--
ALTER TABLE `roles`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `UX_roles_Name` (`Name`);

--
-- Indexes for table `role_permissions`
--
ALTER TABLE `role_permissions`
  ADD PRIMARY KEY (`RoleId`,`PermissionId`),
  ADD KEY `IX_role_permissions_PermissionId` (`PermissionId`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `IX_users_Email` (`Email`),
  ADD KEY `IX_users_RoleId` (`RoleId`);

--
-- Indexes for table `__efmigrationshistory`
--
ALTER TABLE `__efmigrationshistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `certificates`
--
ALTER TABLE `certificates`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT for table `documents`
--
ALTER TABLE `documents`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT for table `fees`
--
ALTER TABLE `fees`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `marriage_applications`
--
ALTER TABLE `marriage_applications`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT for table `marriage_witnesses`
--
ALTER TABLE `marriage_witnesses`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=19;

--
-- AUTO_INCREMENT for table `payments`
--
ALTER TABLE `payments`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT for table `permissions`
--
ALTER TABLE `permissions`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT for table `roles`
--
ALTER TABLE `roles`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `admin_notification_read_states`
--
ALTER TABLE `admin_notification_read_states`
  ADD CONSTRAINT `FK_admin_notification_read_states_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `certificates`
--
ALTER TABLE `certificates`
  ADD CONSTRAINT `FK_certificates_marriage_applications_ApplicationId` FOREIGN KEY (`ApplicationId`) REFERENCES `marriage_applications` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `documents`
--
ALTER TABLE `documents`
  ADD CONSTRAINT `FK_documents_marriage_applications_ApplicationId` FOREIGN KEY (`ApplicationId`) REFERENCES `marriage_applications` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `marriage_applications`
--
ALTER TABLE `marriage_applications`
  ADD CONSTRAINT `FK_marriage_applications_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`);

--
-- Constraints for table `marriage_witnesses`
--
ALTER TABLE `marriage_witnesses`
  ADD CONSTRAINT `FK_marriage_witnesses_marriage_applications_ApplicationId` FOREIGN KEY (`ApplicationId`) REFERENCES `marriage_applications` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `payments`
--
ALTER TABLE `payments`
  ADD CONSTRAINT `FK_payments_fees_FeeId` FOREIGN KEY (`FeeId`) REFERENCES `fees` (`Id`),
  ADD CONSTRAINT `FK_payments_marriage_applications_ApplicationId` FOREIGN KEY (`ApplicationId`) REFERENCES `marriage_applications` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_payments_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`);

--
-- Constraints for table `role_permissions`
--
ALTER TABLE `role_permissions`
  ADD CONSTRAINT `FK_role_permissions_permissions_PermissionId` FOREIGN KEY (`PermissionId`) REFERENCES `permissions` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_role_permissions_roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `users`
--
ALTER TABLE `users`
  ADD CONSTRAINT `FK_users_roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
