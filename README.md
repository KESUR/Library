# Projekt Bazy Danych

## Wprowadzenie
Ten projekt zawiera bazę danych utworzoną przy użyciu phpMyAdmin w środowisku lokalnym. Aby móc korzystać z bazy danych, należy uruchomić XAMPP na swoim komputerze.

## Wymagania
- XAMPP - środowisko do uruchamiania serwera Apache, MySQL oraz interpretera języka PHP. Możesz pobrać XAMPP ze strony [apachefriends.org](https://www.apachefriends.org/index.html).

## Instrukcje uruchomienia
1. Pobierz i zainstaluj XAMPP na swoim komputerze zgodnie z instrukcjami dostępnymi na stronie producenta.
2. Uruchom XAMPP Control Panel.
3. W panelu XAMPP zaznacz moduły Apache oraz MySQL i kliknij przycisk "Start", aby uruchomić serwer Apache oraz serwer MySQL.
4. Otwórz przeglądarkę internetową i przejdź do adresu `http://localhost/phpmyadmin`.
5. Zaloguj się do phpMyAdmin używając domyślnego loginu (zazwyczaj jest to "root") oraz hasła (zazwyczaj puste lub "root").
6. Zaimportuj plik zawierający bazę danych, jeśli istnieje, lub utwórz nową bazę danych.


##KOD DO BAZY DANYCH

-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Czas generowania: 23 Mar 2024, 11:00
-- Wersja serwera: 10.4.27-MariaDB
-- Wersja PHP: 8.2.0

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Baza danych: `pracownicy`
--

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `autorzy`
--

CREATE TABLE `autorzy` (
  `ID` int(11) NOT NULL,
  `firstName` varchar(255) DEFAULT NULL,
  `lastName` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Zrzut danych tabeli `autorzy`
--

INSERT INTO `autorzy` (`ID`, `firstName`, `lastName`) VALUES
(1, 'Jan', 'Kowalski'),
(2, 'Maria', 'Nowak'),
(3, 'Adam', 'Wiśniewski'),
(4, 'Neal', 'Schusterman'),
(5, 'Neal', 'Schusterman'),
(6, 'Neal', 'Schusterman');

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `książki`
--

CREATE TABLE `książki` (
  `id` int(11) NOT NULL,
  `title` varchar(255) DEFAULT NULL,
  `ISBN` varchar(20) DEFAULT NULL,
  `availability` tinyint(1) DEFAULT NULL,
  `authorID` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Zrzut danych tabeli `książki`
--

INSERT INTO `książki` (`id`, `title`, `ISBN`, `availability`, `authorID`) VALUES
(5, 'Kosodom', '12-121-2121-211-1', 1, 4),
(6, 'Susza', '11-111-1111-111-1', 1, 4),
(11, 'Podzieleni', '12-323-2323-33-3', 1, 5),
(12, 'Kosiarze2', '76-172-7266-22-2', 0, 6);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `pracownik`
--

CREATE TABLE `pracownik` (
  `id` int(11) NOT NULL,
  `username` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_polish_ci NOT NULL,
  `password` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_polish_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Zrzut danych tabeli `pracownik`
--

INSERT INTO `pracownik` (`id`, `username`, `password`) VALUES
(1, 'Anna_Nowak', '123456'),
(2, 'Kamil_Kowalski', '121212'),
(3, 'Danuta_Nowa', '654321');

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `przypisane_ksiazki`
--

CREATE TABLE `przypisane_ksiazki` (
  `CzytelnikId` int(255) NOT NULL,
  `fullName` varchar(255) NOT NULL,
  `ISBN` varchar(255) NOT NULL,
  `Tytul` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Zrzut danych tabeli `przypisane_ksiazki`
--

INSERT INTO `przypisane_ksiazki` (`CzytelnikId`, `fullName`, `ISBN`, `Tytul`) VALUES
(1, 'test1', '11-111-1122-211-1', 'Kosiarze'),
(13, 'dsaDSAdsaDSA', '12-121-2121-211-1', 'Kosodom'),
(14, 'dsaDSAdsaDSAxf', '12-323-2323-33-3', 'Podzieleni'),
(15, 'test2', '12-121-2121-211-1', 'Kosodom'),
(16, 'test3', '11-111-1111-111-1', 'Susza'),
(17, 'test5', '12-323-2323-33-3', 'Podzieleni');

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `użytkownik`
--

CREATE TABLE `użytkownik` (
  `id` int(11) NOT NULL,
  `fullName` varchar(100) NOT NULL,
  `phoneNumber` varchar(12) NOT NULL,
  `userName` varchar(255) NOT NULL,
  `password` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Zrzut danych tabeli `użytkownik`
--

INSERT INTO `użytkownik` (`id`, `fullName`, `phoneNumber`, `userName`, `password`) VALUES
(1, 'test1', '+48 21121212', 'test1', 'ed0cb90bdfa4f93981a7d03cff99213a86aa96a6cbcf89ec5e8889871f088727'),
(13, 'dsaDSAdsaDSA', ' dsaDSA32132', 'dsdsdDSAdsaDSAdsa432!', '52ef465b00dbc0d74e8f5ee55646fcdc07a9d7b9f6883f535e6ab360df0fe61b'),
(14, 'dsaDSAdsaDSAxf', '+49 dsaDSA32', 'dsdsdDSAdsaDSAdsa432!1', '09330dbf161f0fc4395b7ac789ea29a087bf48c1530914e2128537f9278aba1c'),
(15, 'test2', '+49 12121212', 'test2', '7e8187582a3350966ed1d759be053f2312087d4b99d4d6c6ddd271152dacefe4'),
(16, 'test3', '+48 12312312', 'test3', '0b58bd431712ff060d06044ad9720ecf9149f80f08ef03ea5b5e209700869c3c'),
(17, 'test5', '+48 12121212', 'test5', '0b58bd431712ff060d06044ad9720ecf9149f80f08ef03ea5b5e209700869c3c');

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `wypożyczenia`
--

CREATE TABLE `wypożyczenia` (
  `id` int(11) NOT NULL,
  `bookID` int(11) DEFAULT NULL,
  `borrowerID` int(11) DEFAULT NULL,
  `borrowDate` date DEFAULT NULL,
  `returnDate` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Indeksy dla zrzutów tabel
--

--
-- Indeksy dla tabeli `autorzy`
--
ALTER TABLE `autorzy`
  ADD PRIMARY KEY (`ID`);

--
-- Indeksy dla tabeli `książki`
--
ALTER TABLE `książki`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `ISBN` (`ISBN`),
  ADD KEY `authorID` (`authorID`);

--
-- Indeksy dla tabeli `pracownik`
--
ALTER TABLE `pracownik`
  ADD PRIMARY KEY (`id`);

--
-- Indeksy dla tabeli `przypisane_ksiazki`
--
ALTER TABLE `przypisane_ksiazki`
  ADD PRIMARY KEY (`CzytelnikId`);

--
-- Indeksy dla tabeli `użytkownik`
--
ALTER TABLE `użytkownik`
  ADD PRIMARY KEY (`id`);

--
-- Indeksy dla tabeli `wypożyczenia`
--
ALTER TABLE `wypożyczenia`
  ADD PRIMARY KEY (`id`),
  ADD KEY `bookID` (`bookID`),
  ADD KEY `borrowerID` (`borrowerID`);

--
-- AUTO_INCREMENT dla zrzuconych tabel
--

--
-- AUTO_INCREMENT dla tabeli `autorzy`
--
ALTER TABLE `autorzy`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT dla tabeli `książki`
--
ALTER TABLE `książki`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT dla tabeli `pracownik`
--
ALTER TABLE `pracownik`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT dla tabeli `przypisane_ksiazki`
--
ALTER TABLE `przypisane_ksiazki`
  MODIFY `CzytelnikId` int(255) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=18;

--
-- AUTO_INCREMENT dla tabeli `użytkownik`
--
ALTER TABLE `użytkownik`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=18;

--
-- AUTO_INCREMENT dla tabeli `wypożyczenia`
--
ALTER TABLE `wypożyczenia`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- Ograniczenia dla zrzutów tabel
--

--
-- Ograniczenia dla tabeli `książki`
--
ALTER TABLE `książki`
  ADD CONSTRAINT `książki_ibfk_1` FOREIGN KEY (`authorID`) REFERENCES `autorzy` (`ID`);

--
-- Ograniczenia dla tabeli `wypożyczenia`
--
ALTER TABLE `wypożyczenia`
  ADD CONSTRAINT `wypożyczenia_ibfk_1` FOREIGN KEY (`bookID`) REFERENCES `książki` (`id`),
  ADD CONSTRAINT `wypożyczenia_ibfk_2` FOREIGN KEY (`borrowerID`) REFERENCES `użytkownik` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
