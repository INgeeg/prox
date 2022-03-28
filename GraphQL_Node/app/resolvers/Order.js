"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __param = (this && this.__param) || function (paramIndex, decorator) {
    return function (target, key) { decorator(target, key, paramIndex); }
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.OrderResolver = void 0;
const type_graphql_1 = require("type-graphql");
const Order_1 = require("../entities/Order");
const Product_1 = require("../entities/Product");
let OrderResolver = class OrderResolver {
    returnSingleProduct(id) {
        return __awaiter(this, void 0, void 0, function* () {
            return yield Order_1.OrderModel.findById({ _id: id });
        });
    }
    returnAllOrder() {
        return __awaiter(this, void 0, void 0, function* () {
            return yield Order_1.OrderModel.find();
        });
    }
    createOrder({ user_id, date, payde }) {
        return __awaiter(this, void 0, void 0, function* () {
            const order = (yield Order_1.OrderModel.create({
                user_id,
                date,
                payde,
            })).save();
            return order;
        });
    }
    deleteOrder(id) {
        return __awaiter(this, void 0, void 0, function* () {
            yield Order_1.OrderModel.deleteOne({ id });
            return true;
        });
    }
    products(order) {
        return __awaiter(this, void 0, void 0, function* () {
            console.log(order, 'order!');
            return (yield Product_1.ProductModel.findById(order._doc.products));
        });
    }
};
__decorate([
    (0, type_graphql_1.Query)((_returns) => Order_1.Order, { nullable: false }),
    __param(0, (0, type_graphql_1.Arg)('id'))
], OrderResolver.prototype, "returnSingleProduct", null);
__decorate([
    (0, type_graphql_1.Query)(() => [Order_1.Order])
], OrderResolver.prototype, "returnAllOrder", null);
__decorate([
    (0, type_graphql_1.Mutation)(() => Order_1.Order),
    __param(0, (0, type_graphql_1.Arg)('data'))
], OrderResolver.prototype, "createOrder", null);
__decorate([
    (0, type_graphql_1.Mutation)(() => Boolean),
    __param(0, (0, type_graphql_1.Arg)('id'))
], OrderResolver.prototype, "deleteOrder", null);
__decorate([
    (0, type_graphql_1.FieldResolver)((_type) => Product_1.Product),
    __param(0, (0, type_graphql_1.Root)())
], OrderResolver.prototype, "products", null);
OrderResolver = __decorate([
    (0, type_graphql_1.Resolver)((_of) => Order_1.Order)
], OrderResolver);
exports.OrderResolver = OrderResolver;
