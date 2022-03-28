"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.CartModel = exports.Cart = void 0;
const type_graphql_1 = require("type-graphql");
const typegoose_1 = require("@typegoose/typegoose");
const Product_1 = require("./Product");
let Cart = class Cart {
};
__decorate([
    (0, type_graphql_1.Field)(() => type_graphql_1.ID)
], Cart.prototype, "id", void 0);
__decorate([
    (0, type_graphql_1.Field)((_type) => String),
    (0, typegoose_1.prop)({ ref: Product_1.Product, required: true })
], Cart.prototype, "products", void 0);
Cart = __decorate([
    (0, type_graphql_1.ObjectType)({ description: 'The  Cart model' })
], Cart);
exports.Cart = Cart;
exports.CartModel = (0, typegoose_1.getModelForClass)(Cart);
