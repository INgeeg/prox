"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.ProductModel = exports.Product = void 0;
const type_graphql_1 = require("type-graphql");
const typegoose_1 = require("@typegoose/typegoose");
const Categories_1 = require("./Categories");
let Product = class Product {
};
__decorate([
    (0, type_graphql_1.Field)(() => type_graphql_1.ID)
], Product.prototype, "id", void 0);
__decorate([
    (0, type_graphql_1.Field)(),
    (0, typegoose_1.prop)()
], Product.prototype, "name", void 0);
__decorate([
    (0, type_graphql_1.Field)(),
    (0, typegoose_1.prop)()
], Product.prototype, "description", void 0);
__decorate([
    (0, type_graphql_1.Field)(),
    (0, typegoose_1.prop)()
], Product.prototype, "color", void 0);
__decorate([
    (0, type_graphql_1.Field)((_type) => type_graphql_1.Int),
    (0, typegoose_1.prop)()
], Product.prototype, "stock", void 0);
__decorate([
    (0, type_graphql_1.Field)((_type) => type_graphql_1.Int),
    (0, typegoose_1.prop)()
], Product.prototype, "price", void 0);
__decorate([
    (0, type_graphql_1.Field)((_type) => String),
    (0, typegoose_1.prop)({ ref: Categories_1.Categories })
], Product.prototype, "category_id", void 0);
Product = __decorate([
    (0, type_graphql_1.ObjectType)({ description: 'The Product model' })
], Product);
exports.Product = Product;
exports.ProductModel = (0, typegoose_1.getModelForClass)(Product);
