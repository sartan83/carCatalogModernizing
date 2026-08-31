# Automotive catalog — WinForms + WCF sample

> ### DISCLAIMER
> **IMPORTANT:** The current state of this sample application is **BETA**, consider it version a 0.01, therefore, many areas will be improved and changed significantly. This is purely built with the purpose of showing off a concept/demo at this point.

![WinForms](assets/winForms.PNG)

## Introduction

This is a traditional Line of Business WinForms application. The front-end is a fictional inventory app which allows its users to track inventory for the cars and spare parts being sold through their dealership. You can look at inventory counts for particular items at different calendar dates.

## Repository and Project Structure

The solution lives in `eShopLegacyNTier`: WinForms (no High DPI support) interfacing with a local WCF service which talks to a local SQL database. Each component is broken into its own project (frontend, WCF, etc).
