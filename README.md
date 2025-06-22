# 🍳 TasteBud Recipes Platform

![Project Banner](https://via.placeholder.com/1200x400/FF6B6B/FFFFFF?text=TasteBud+Recipes+Platform)

A modern web platform for sharing, discovering, and rating recipes with a clean UI and intuitive user experience.

## 📋 Table of Contents
- [SRS Documentation](#-srs-documentation)
- [Database Design](#-database-design)
- [UI/UX Wireframes](#-uiux-wireframes)
- [Getting Started](#-getting-started)
- [License](#-license)

## 📄 SRS Documentation

### 1. Introduction
**Purpose**: Web-based platform for recipe sharing with community ratings  
**Scope**:  
- User registration and recipe management  
- Search functionality by category/ingredients  
- 5-star rating system  
- Admin content moderation  

[View Full SRS Document](./docs/SRS.md)

### 2. Functional Requirements
| Feature          | Description                                |
|------------------|--------------------------------------------|
| User Registration| Email/password (8+ chars)                 |
| Recipe Creation  | Title, ingredients, steps, category       |
| Search           | Filter by keyword/category/difficulty     |
| Rating System    | 1-5 stars per user per recipe             |

### 3. Non-Functional Requirements
| Category     | Requirement                          |
|-------------|--------------------------------------|
| Performance | Page load <3s, 500+ concurrent users |
| Security    | Password hashing + HTTPS             |
| Usability   | Mobile-responsive design             |

## 🗃️ Database Design

### ER Diagram
```mermaid
erDiagram
    USER ||--o{ RECIPE : "creates"
    USER ||--o{ RATING : "submits"
    RECIPE ||--o{ RATING : "receives"
    RECIPE }o--|| CATEGORY : "belongs_to"
  
