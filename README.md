# RaceDay - Event Management System

## Overview
RaceDay is a full-stack web-based event management system for the South African road running, walking, and cycling community. Built as part of PROG6212 POE.

## System Roles
1. **Organiser** - Create, edit, and delete events; manage categories; capture results
2. **Participant** - Browse events; enter events; track personal results

## Part 1 - Planning & Database
- **ERD**: `/docs/ERD.png` - 8 entities with full relationships
- **API Endpoint Plan**: `/docs/API_Endpoint_Plan.md` - 25+ RESTful endpoints
- **SQL Script**: `/docs/RaceDay_Database.sql` - Complete schema with seed data

## CI/CD Status
![CI/CD Build](https://github.com/yourusername/raceday/actions/workflows/validate-docs.yml/badge.svg)

## Video Presentation
[Unlisted YouTube Link](https://youtu.be/your-video-link)

## Setup Instructions
1. Clone repository
2. Open SQL Server Management Studio (SSMS)
3. Run `/docs/RaceDay_Database.sql` on a clean SQL Server instance
4. Verify 8 tables are created with seed data
5. CI/CD workflow will validate automatically on push
