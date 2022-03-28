"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.OrderModel = exports.Order = void 0;
const type_graphql_1 = require("type-graphql");
const typegoose_1 = require("@typegoose/typegoose");
const Product_1 = require("./Product");
let Order = class Order {
};
__decorate([
    (0, type_graphql_1.Field)(() => type_graphql_1.ID)
], Order.prototype, "id", void 0);
__decorate([
    (0, type_graphql_1.Field)(),
    (0, typegoose_1.prop)({ nullable: true })
], Order.prototype, "user_id", void 0);
__decorate([
    (0, type_graphql_1.Field)(),
    (0, typegoose_1.prop)({ required: true })
], Order.prototype, "payde", void 0);
__decorate([
    (0, type_graphql_1.Field)(),
    (0, typegoose_1.prop)({ default: new Date(), required: true, nullable: true })
], Order.prototype, "date", void 0);
__decorate([
    (0, typegoose_1.prop)({ ref: Product_1.Product, required: true })
], Order.prototype, "products", void 0);
Order = __decorate([
    (0, type_graphql_1.ObjectType)({ description: 'The Order model' })
], Order);
exports.Order = Order;
exports.OrderModel = (0, typegoose_1.getModelForClass)(Order);
